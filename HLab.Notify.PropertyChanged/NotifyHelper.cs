using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HLab.Notify.PropertyChanged.PropertyHelpers;

namespace HLab.Notify.PropertyChanged;

[AttributeUsage(AttributeTargets.Class)]
public class DeepNotifyActivation : Attribute
{

}

public interface INotifyHelper<TClass> where TClass : class, INotifyPropertyChangedWithHelper
{
    public H<TClass> Helper => H<TClass>.Helper;
}

public class H<TClass> : NotifyHelper
    where TClass : class, INotifyPropertyChangedWithHelper
{
    public static H<TClass> Helper { get; } = new();

    internal static Lazy<Action<TClass>> InitializeAction { get; } = new (() => CreateActivatorA() + CreateActivatorExt());

    internal static bool HasActivator => InitializeAction.IsValueCreated;

    public static void Initialize(TClass target)
    {
        target.ClassHelper.Initialize<TClass>();
    }

    // TRIGGER
    public static ITrigger Trigger(NotifyConfiguratorFactory<TClass, NotifyTrigger> configurator, [CallerMemberName] string name = null)
    {
        var c =
            PropertyCache<TClass>.GetByHolder<NotifyTrigger>(name,
                n => configurator(new NotifyConfigurator<TClass, NotifyTrigger>())
                    .Compile(n)
            );
        return new NotifyTrigger(c);
    }


    /************
     * PROPERTY *
     ************/
#if DUMMYPROPERTIES
        public static IProperty<T> Property<T>(string dummy) => null;
        public static IProperty<T> Property<T>(NotifyConfiguratorFactory<TClass, PropertyHolder<T>> configurator, string dummy) => null;
        public static IProperty<T> Property<T>() => null;
        public static IProperty<T> Property<T>(NotifyConfiguratorFactory<TClass, PropertyHolder<T>> configurator) => null;
#else
    public static IProperty<T> Property<T>(NotifyConfiguratorFactory<TClass, IProperty<T>> configurator, [CallerMemberName] string name = null)
    {
        // TODO : benchmark this and see if just storing uncompiled whould perform better :
        var activator =
            PropertyCache<TClass>.GetByHolder<IProperty<T>>(name,
                n => configurator(new NotifyConfigurator<TClass, IProperty<T>>())
                    .Compile(n)
            );

        return PropertyValueHolder<T>.Create(activator);

        return new PropertyHolder<T>(activator);
    }
    public static PropertyHolder<T> PropertyUncompiled<T>(NotifyConfiguratorFactory<TClass, PropertyHolder<T>> configurator, [CallerMemberName] string name = null)
    {
        // TODO : benchmark this and see if just storing uncompiled whould perform better :
        var activator =
//                PropertyCache<TClass>.GetByHolder<PropertyHolder<T>>(name,n => 
                configurator(new NotifyConfigurator<TClass, PropertyHolder<T>>())
                    .Compile(name)
//                )
            ;
        return new PropertyHolder<T>(activator);
    }
    public static IProperty<T> Property<T>([CallerMemberName] string name = null) => Property<T>(c => c, name);
#endif
    public static IProperty<T> BindProperty<T>(Expression<Func<TClass, T>> expr, [CallerMemberName] string name = null) =>  Property<T>(c => c.Bind(expr), name);


    /***********
     * COMMAND *
     ***********/
    public static ICommand Command(NotifyConfiguratorFactory<TClass, CommandPropertyHolder> configurator, [CallerMemberName] string name = null)
    {
#if DEBUG
        Debug.Assert(typeof(TClass).GetProperty(name) != null, $"{name} is not a property of " + typeof(TClass).Name + " (did you forget {get;} ?)");
#endif
        var c = PropertyCache<TClass>.GetByHolder<CommandPropertyHolder>(name,
            n => configurator(new NotifyConfigurator<TClass, CommandPropertyHolder>())
                .Compile(n));

        return new CommandPropertyHolder(c);
    }

    // Filter //
    public static IObservableFilter<T> Filter<T>(NotifyConfiguratorFactory<TClass, ObservableFilterPropertyHolder<T>> configurator, [CallerMemberName] string name = null)
    {
        var c = PropertyCache<TClass>.GetByHolder<ObservableFilterPropertyHolder<T>>(name,
            n => configurator(new NotifyConfigurator<TClass, ObservableFilterPropertyHolder<T>>().Init((target, filter) => filter.OnTriggered()))
                .Compile(n));

        return new ObservableFilterPropertyHolder<T>(c);
    }

    struct Todo
    {
        public string Name;
        public MemberInfo MemberInfo;
        public PropertyActivator Activator;
        public override string ToString() => Name;
    }

    static bool CheckDependencies(Todo todo, ICollection<string> done, Queue<Todo> todoQueue)
    {
        var name = todo.MemberInfo.Name;
        var a = PropertyCache<TClass>.GetByHolder(name);
        if (a == null)
        {
            done.Add(name);
            return true;
        }
        //            var p = todo.ConfiguratorEntry;
        if (a.DependsOn
            .Where(d => !done.Contains(d))
            .Any(d => todoQueue.Any(i => i.Name == d))
           )
        {
            todoQueue.Enqueue(todo);
            return false;
        }
        done.Add(a.PropertyName);
        return true;
    }

    static Action<TClass> CreateActivatorA()
    {
        var todoList = new Queue<Todo>();
        var done = new List<string>();

        var type = typeof(TClass);

        while (true)

        {
            //                var getByHolder = typeof(PropertyCache<>).MakeGenericType(type).GetMethod("GetByHolder",new []{typeof(string)});

            foreach (var mi in type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance |
                                               BindingFlags.NonPublic | BindingFlags.Public))
            {
                switch (mi)
                {
                    case FieldInfo fieldInfo:
                        if (!typeof(IChildObject).IsAssignableFrom(fieldInfo.FieldType)) continue;
                        if (mi.Name.Contains("k__BackingField")) continue;
                        var a = PropertyCache<TClass>.GetByHolder(mi.Name);
                        //var p = (ConfiguratorEntry) getByHolder.Invoke(null,new object[]{mi.Name});
                        todoList.Enqueue(new Todo { MemberInfo = mi, Name = a.PropertyName, Activator = a });
                        break;
                    case PropertyInfo propertyInfo:
                        //if (propertyInfo.CanWrite) continue;
                        if (propertyInfo.GetMethod.IsAbstract) continue;

                        if (typeof(IChildObject).IsAssignableFrom(propertyInfo.PropertyType))
                        {
                            todoList.Enqueue(new Todo { MemberInfo = mi, Name = mi.Name });
                        }
                        else if (typeof(ICommand).IsAssignableFrom(propertyInfo.PropertyType))
                        {
                            todoList.Enqueue(new Todo { MemberInfo = mi, Name = mi.Name });
                        }
                        break;
                }
            }

            type = type.BaseType;
            if (type != null && typeof(INotifyPropertyChangedWithHelper).IsAssignableFrom(type))
            {
                var t = typeof(H<>);
                var t1 = t.MakeGenericType(type);
                var p = t1.GetProperty("HasActivator", BindingFlags.NonPublic | BindingFlags.Static);

                if (p != null)
                {
                    var hasActivator = (bool)p.GetValue(null);

                    if (hasActivator) break;

                }
                else
                {

                }

            }
            else break;
        }

        Action<TClass> activator = null;

        while (todoList.TryDequeue(out var todo))
        {
            switch (todo.MemberInfo)
            {
                case FieldInfo fieldInfo:
                    if (!typeof(IChildObject).IsAssignableFrom(fieldInfo.FieldType)) continue;
                    if (!CheckDependencies(todo, done, todoList)) continue;
                    activator += t =>
                    {
                        ((IChildObject)fieldInfo.GetValue(t))
                            .Parent = t;
                    };
                    break;
                case PropertyInfo propertyInfo:
                    if (propertyInfo.CanWrite) continue;

                    if (typeof(IChildObject).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        if (!CheckDependencies(todo, done, todoList)) continue;
                        activator += t =>
                        {
                            var pi = propertyInfo;
                            var value = propertyInfo.GetValue(t);
                            if (value is IChildObject child)
                            {
                                child.Parent = t;
                            }
                            else
                            {

                            }
                        };
                    }
                    else if (typeof(ICommand).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        if (!CheckDependencies(todo, done, todoList)) continue;
                        activator += t =>
                        {
                            if (propertyInfo.GetValue(t) is IChildObject child)
                            {
                                child.Parent = t;
                            }
                        };

                    }
                    break;
            }
        }
        return activator;
    }

    static Action<TClass> CreateActivator()
    {
        DynamicMethod dm =
            new DynamicMethod(
                "activate",
                typeof(void),
                new Type[]
                {
                    typeof(TClass),
                    typeof(INotifyClassHelper),
                    typeof(Action<PropertyChangedEventArgs>)
                },
                typeof(TClass), true);


        ILGenerator il = dm.GetILGenerator();

        var type = typeof(TClass);
        //while (type != null)
        {
            foreach (var f in type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance |
                                              BindingFlags.NonPublic | BindingFlags.Public))
            {
                switch (f)
                {
                    case FieldInfo fieldInfo:
                        if (!typeof(IChildObject).IsAssignableFrom(fieldInfo.FieldType)) continue;
                        // Load the instance of the object (argument 0) onto the stack
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Castclass, type);
                        // Load the value of the object's field (fi) onto the stack
                        il.Emit(OpCodes.Ldfld, fieldInfo);

                        il.Emit(OpCodes.Castclass, typeof(IChildObject));
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Ldarg_2);
                        il.Emit(OpCodes.Callvirt, SetParent);
                        break;
                    case PropertyInfo propertyInfo:
                        if (!typeof(IChildObject).IsAssignableFrom(propertyInfo.PropertyType)) continue;
                        if (propertyInfo.CanWrite) continue;
                        // Load the instance of the object (argument 0) onto the stack
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Castclass, type);
                        // Load the value of the object's field (fi) onto the stack
                        if (propertyInfo.GetMethod.IsVirtual)
                            il.Emit(OpCodes.Callvirt, propertyInfo.GetMethod);
                        else
                            il.Emit(OpCodes.Call, propertyInfo.GetMethod);

                        il.Emit(OpCodes.Castclass, typeof(IChildObject));
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Ldarg_2);
                        il.Emit(OpCodes.Callvirt, SetParent);
                        break;
                }


                //var returnType = f.GetReturnType();
            }

            type = type.BaseType;
        }

        // return the value on the top of the stack
        il.Emit(OpCodes.Ret);

        var d = dm.CreateDelegate(typeof(Action<TClass, Action<PropertyChangedEventArgs>>));

        return (Action<TClass>)d;
    }

    static Action<TClass> CreateActivatorExt()
    {

        Action<TClass> activator = null;

        foreach (var property in typeof(TClass).GetProperties(
                     BindingFlags.Public |
                     BindingFlags.NonPublic |
                     BindingFlags.DeclaredOnly |
                     BindingFlags.Instance))
        {
            foreach (var attribute in property.GetCustomAttributes().OfType<TriggerOnAttribute>())
            {
                if (typeof(ITriggerable).IsAssignableFrom(property.PropertyType))
                {
                    activator += o =>
                    {
                        attribute.Path.GetTrigger(o.ClassHelper,
                            (s, args) => (property.GetValue(o) as ITriggerable)?.OnTriggered());
                    };
                }
                else
                {
                    activator += o =>
                    {
                        attribute.Path.GetTrigger(o.ClassHelper,
                            (s, args) =>
                                o.ClassHelper.OnPropertyChanged(new PropertyChangedEventArgs(property.Name)));
                    };
                }
            }
        }

        foreach (var method in typeof(TClass).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance))
        {
            foreach (var attribute in method.GetCustomAttributes().OfType<TriggerOnAttribute>())
            {
                activator += o =>
                {
                    attribute.Path.GetTrigger(o.ClassHelper, (s, args) => method.Invoke(o, new object[] { }));
                };
            }
        }


        activator += o =>
        {
            var l = o.ClassHelper./*Linked*/Properties().ToList();
            foreach (var property in l)
            {
                property.InitialRegisterValue(typeof(TClass));
            }
        };

        return activator;

    }
}