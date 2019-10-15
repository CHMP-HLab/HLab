using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HLab.DependencyInjection;
using HLab.Notify.Annotations;

namespace HLab.Notify.PropertyChanged
{

    public class NotifyHelper<TClass> : NotifyHelper
        where TClass : class//,INotifyPropertyChanged
    {
        public delegate void NotifyActivator(TClass target, INotifyClassParser parser,
            Action<PropertyChangedEventArgs> evt);

        private static readonly NotifyActivator InitializeAction = CreateActivatorA() + CreateActivatorExt();
        public static void Initialize(TClass target,Action<PropertyChangedEventArgs> callback=null) 
            => InitializeAction(target,NotifyFactory.GetParser(target),callback);
        // PROPERTY
        public static PropertyHolder<T> Property<T>(string name, NotifyConfiguratorFactory<TClass, PropertyHolder<TClass,T>> c)
        {
            return new PropertyHolder<TClass, T>(name, c);
        }

        public static PropertyHolder<T> Property<T>(NotifyConfiguratorFactory<TClass, PropertyHolder<TClass,T>> c, [CallerMemberName]string name = null)
            => Property(Name(name), c);
        
        public static IProperty<T> Property<T>([CallerMemberName]string name = null) => Property<T>(Name(name),c => c);

        // COMMAND 

        public static ICommand Command(NotifyConfiguratorFactory<TClass, NotifyCommand<TClass>> c, [CallerMemberName]string name = null)
        => new NotifyCommand<TClass>(name,c);

        public static IObservableFilter<T> Filter<T>(Func<TClass, ObservableFilter<TClass, T>, ObservableFilter<TClass, T>> configurator)
            => new ObservableFilter<TClass,T>(configurator);

        private static NotifyActivator CreateActivatorA()
        {
            NotifyActivator activator = (a, b, c) => { };

            var type = typeof(TClass);
            while (type != null)
            {
                foreach (var f in type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance |
                                                  BindingFlags.NonPublic | BindingFlags.Public))
                {
                    switch (f)
                    {
                        case FieldInfo fieldInfo:
                            if (!typeof(IChildObject).IsAssignableFrom(fieldInfo.FieldType)) continue;

                            activator += (t, p, e) =>
                            {
                                var child =(IChildObject)fieldInfo.GetValue(t);
                                child.SetParent(t,p,e);
                            };
                            break;
                        case PropertyInfo propertyInfo:
                            if (propertyInfo.CanWrite) continue;
                            if (typeof(IChildObject).IsAssignableFrom(propertyInfo.PropertyType))
                            {
                                activator += (t, p, e) =>
                                {
                                    var child = (IChildObject) propertyInfo.GetValue(t);
                                    child.SetParent(t, p, e);
                                };
                            }
                            else if (typeof(ICommand).IsAssignableFrom(propertyInfo.PropertyType))
                            {
                                activator += (t, p, e) =>
                                {
                                    if (propertyInfo.GetValue(t) is IChildObject child)
                                    {
                                        child.SetParent(t, p, e);
                                    }
                                };

                            }
                            break;
                    }


                    var returnType = f.GetReturnType();


                }

                type = type.BaseType;
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
                        typeof(INotifyClassParser),
                        typeof(Action<PropertyChangedEventArgs>)
                    },
                    typeof(TClass), true);


            ILGenerator il = dm.GetILGenerator();

            var type = typeof(TClass);
            while (type != null)
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

            NotifyActivator activator = (a, b, c) => { };

            var type = typeof(TClass);
            while (type != null)
            {
                foreach (var property in type.GetProperties(
                    BindingFlags.Public | 
                    BindingFlags.NonPublic | 
                    BindingFlags.DeclaredOnly | 
                    BindingFlags.Instance))
                {
                    foreach (var attribute in property.GetCustomAttributes().OfType<TriggerOnAttribute>())
                    {
                        if (typeof(ITriggerable).IsAssignableFrom(property.PropertyType))
                        {
                            activator += (o,p,a) =>
                            {
                                NotifyFactory.GetParser(o).GetTrigger(attribute.Path, (s, args) => (property.GetValue(o) as ITriggerable)?.OnTriggered());
                            };
                        }
                        else
                        {
                            activator += (o,p,a) =>
                            {
                                NotifyFactory.GetParser(o).GetTrigger(attribute.Path, (s, args) => a(new PropertyChangedEventArgs(property.Name)));
                            };
                        }
                    }
                }

                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance))
                {
                    foreach (var attribute in method.GetCustomAttributes().OfType<TriggerOnAttribute>())
                    {
                        activator += (o,p,a) =>
                        {
                            NotifyFactory.GetParser(o).GetTrigger(attribute.Path, (s, args) => method.Invoke(o, new object[] { }));
                        };
                    }
                }

                type = type.BaseType;
            }

            activator += (o,p,a) =>
            {
                var l = NotifyFactory.GetExistingParser(o)?.LinkedProperties().ToList();
                if (l == null) return;
                foreach (var property in l)
                {
                    property.InitialRegisterValue();
                }
            };

            return activator;

        }
    }
}