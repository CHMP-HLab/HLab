using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HLab.DependencyInjection;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged.NotifyParsers;
using HLab.Notify.PropertyChanged.PropertyHelpers;

namespace HLab.Notify.PropertyChanged
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DeepNotifyActivation : Attribute
    {

    }

    public class H<TClass> : NotifyHelper
        where TClass : class, INotifyPropertyChanged
    {
        public delegate void NotifyActivator(TClass target, INotifyClassHelper parser);

        internal static readonly Lazy<NotifyActivator> InitializeAction = new Lazy<NotifyActivator>(()=>CreateActivatorA() + CreateActivatorExt());
        public static void Initialize(TClass target)
        {
            NotifyClassHelper.GetHelper(target).Initialize<TClass>();
        }

        // TRIGGER
        public static ITrigger Trigger(NotifyConfiguratorFactory<TClass, NotifyTrigger> configurator, [CallerMemberName]string name = null)
        {
            
            var c=
                    PropertyCache<TClass>.GetByHolder<NotifyTrigger>(name,
                        n => configurator(new NotifyConfigurator<TClass, NotifyTrigger>())
                            .Compile(n)
                    );
            return new NotifyTrigger(c);
        }


        // PROPERTY
        #if DUMMYPROPERTIES
        public static IProperty<T> Property<T>(string dummy) => null;
        public static IProperty<T> Property<T>(NotifyConfiguratorFactory<TClass, PropertyHolder<T>> configurator, string dummy) => null;
        public static IProperty<T> Property<T>() => null;
        public static IProperty<T> Property<T>(NotifyConfiguratorFactory<TClass, PropertyHolder<T>> configurator) => null;
        #else
         public static PropertyHolder<T> Property<T>(NotifyConfiguratorFactory<TClass, PropertyHolder<T>> configurator, [CallerMemberName] string name = null)
        {
            // TODO : benchmark this and see if just storing uncompiled whould perform better :
            var c =
                    PropertyCache<TClass>.GetByHolder<PropertyHolder<T>>(name,
                        n => configurator(new NotifyConfigurator<TClass, PropertyHolder<T>>())
                            .Compile(n)
                    );
            return new PropertyHolder<T>(c);
        }
        public static IProperty<T> Property<T>([CallerMemberName]string name = null) => Property<T>(c => c,name);
        #endif


        // COMMAND 

        public static ICommand Command(NotifyConfiguratorFactory<TClass, NotifyCommand> configurator, [CallerMemberName]string name = null)
        {
            #if DEBUG
            Debug.Assert(typeof(TClass).GetProperty(name)!=null,$"{name} is not a property of " + typeof(TClass).Name);
            #endif
            var c =  PropertyCache<TClass>.GetByHolder<NotifyCommand>(name,
                        n => configurator(new NotifyConfigurator<TClass, NotifyCommand>())
                            .Compile(n));

            return new NotifyCommand(c);
        } 

        public static IObservableFilter<T> Filter<T>(NotifyConfiguratorFactory<TClass, ObservableFilter<T>> configurator, [CallerMemberName]string name = null)
        {
            var c =  PropertyCache<TClass>.GetByHolder<ObservableFilter<T>>(name,
                        n => configurator(new NotifyConfigurator<TClass, ObservableFilter<T>>().Init((target,filter)=>filter.OnTriggered()))
                            .Compile(n));

            return new ObservableFilter<T>(c);
        }

        struct Todo
        {
            public string Name;
            public MemberInfo MemberInfo;
            public override string ToString() => Name;
        }

        private static bool CheckDependencies(Todo todo, List<string> done, Queue<Todo> todoQueue)
        {
            var p = PropertyCache<TClass>.GetByHolder(todo.MemberInfo.Name);
            foreach (var d in p.Dependencies)
            {
                if (!done.Contains(d))
                {
                    if (todoQueue.Any(i => i.Name == d))
                    {
                        todoQueue.Enqueue(todo);
                        return false;
                    }
                }
            }
            done.Add(p.EventArgs.PropertyName);
            return true;
        }

        private static NotifyActivator CreateActivatorA()
        {
            NotifyActivator activator = null;
            var todoList = new Queue<Todo>();
            var done = new List<string>();

            foreach (var mi in typeof(TClass).GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance |
                                                         BindingFlags.NonPublic | BindingFlags.Public))
            {
                switch (mi)
                {
                    case FieldInfo fieldInfo:
                        if (!typeof(IChildObject).IsAssignableFrom(fieldInfo.FieldType)) continue;
                        if (mi.Name.Contains("k__BackingField")) continue;
                        var p = PropertyCache<TClass>.GetByHolder(mi.Name);
                        todoList.Enqueue(new Todo{MemberInfo = mi,Name = p.EventArgs.PropertyName});
                        break;
                    case PropertyInfo propertyInfo:
                        if (propertyInfo.CanWrite) continue;

                            
                        if (typeof(IChildObject).IsAssignableFrom(propertyInfo.PropertyType))
                        {
                            todoList.Enqueue(new Todo{MemberInfo = mi,Name = mi.Name});
                        }
                        else if (typeof(ICommand).IsAssignableFrom(propertyInfo.PropertyType))
                        {
                            todoList.Enqueue(new Todo{MemberInfo = mi,Name = mi.Name});
                        }
                        break;
                }
            }


            while (todoList.TryDequeue(out var todo))
            {
                switch (todo.MemberInfo)
                    {
                        case FieldInfo fieldInfo:
                            if (!typeof(IChildObject).IsAssignableFrom(fieldInfo.FieldType)) continue;
                            if (!CheckDependencies(todo, done, todoList)) continue;
                            activator += (t, p) =>
                            {
                                var child =(IChildObject)fieldInfo.GetValue(t);
                                child.SetParent(t,p);
                            };
                            break;
                        case PropertyInfo propertyInfo:
                            if (propertyInfo.CanWrite) continue;
                            
                            if (typeof(IChildObject).IsAssignableFrom(propertyInfo.PropertyType))
                            {
                                if (!CheckDependencies(todo, done, todoList)) continue;
                                activator += (t, p) =>
                                {
                                    var child = (IChildObject) propertyInfo.GetValue(t);
                                    child.SetParent(t, p);
                                };
                            }
                            else if (typeof(ICommand).IsAssignableFrom(propertyInfo.PropertyType))
                            {
                                if (!CheckDependencies(todo, done, todoList)) continue;
                                activator += (t, p) =>
                                {
                                    if (propertyInfo.GetValue(t) is IChildObject child)
                                    {
                                        child.SetParent(t, p);
                                    }
                                };

                            }
                            break;
                    }
            }
            return activator;
        }

        private static NotifyActivator CreateActivator()
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


                    var returnType = f.GetReturnType();


                }

                type = type.BaseType;
            }

            // return the value on the top of the stack
            il.Emit(OpCodes.Ret);

            var d = dm.CreateDelegate(typeof(Action<TClass, Action<PropertyChangedEventArgs>>));

            return d as NotifyActivator;
        }

        private static NotifyActivator CreateActivatorExt()
        {

            NotifyActivator activator = null;

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
                        activator += (o,p) =>
                        {
                            p.GetTrigger(attribute.Path, (s, args) => (property.GetValue(o) as ITriggerable)?.OnTriggered());
                        };
                    }
                    else
                    {
                        activator += (o,p) =>
                        {
                            p.GetTrigger(attribute.Path, (s, args) => p.OnPropertyChanged(new PropertyChangedEventArgs(property.Name)));
                        };
                    }
                }
            }

            foreach (var method in typeof(TClass).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance))
            {
                foreach (var attribute in method.GetCustomAttributes().OfType<TriggerOnAttribute>())
                {
                    activator += (o,p) =>
                    {
                        p.GetTrigger(attribute.Path, (s, args) => method.Invoke(o, new object[] { }));
                    };
                }
            }


            activator += (o,p) =>
            {
                var l = p./*Linked*/Properties().ToList();
                foreach (var property in l)
                {
                    property.InitialRegisterValue(typeof(TClass));
                }
            };

            return activator;

        }
    }
}