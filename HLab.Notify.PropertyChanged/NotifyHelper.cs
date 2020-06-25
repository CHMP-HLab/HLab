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
        public delegate void NotifyActivator(TClass target, INotifyClassParser parser);

        internal static readonly NotifyActivator InitializeAction = CreateActivatorA() + CreateActivatorExt();
        public static void Initialize(TClass target)
        {
            NotifyFactory.GetParser(target).Initialize<TClass>();
            //InitializeAction(target, parser, callback);
        }
        // TRIGGER
        public static ITrigger Trigger(string name, NotifyConfiguratorFactory<TClass, NotifyTrigger> configurator)
        {
            
            var c=
                    PropertyCache<TClass>.Get(name,
                        () => configurator(new NotifyConfigurator<TClass, NotifyTrigger>())
                            .Compile()
                    );
            return new NotifyTrigger(c);
        }
        public static ITrigger Trigger(NotifyConfiguratorFactory<TClass, NotifyTrigger> c, [CallerMemberName]string name = null)
            => Trigger(Name(name), c);


        // PROPERTY
        public static PropertyHolder<T> Property<T>(string name, NotifyConfiguratorFactory<TClass, PropertyHolder<T>> configurator)
        {
            
            var c=
                    PropertyCache<TClass>.Get(name,
                        () => configurator(new NotifyConfigurator<TClass, PropertyHolder<T>>())
                            .Compile()
                    );
            return new PropertyHolder<T>(c);
        }

        public static PropertyHolder<T> Property<T>(NotifyConfiguratorFactory<TClass, PropertyHolder<T>> c, [CallerMemberName]string name = null)
            => Property(Name(name), c);


        public static IProperty<T> Property<T>([CallerMemberName]string name = null) => Property<T>(Name(name),c => c);

        // COMMAND 

        public static ICommand Command(NotifyConfiguratorFactory<TClass, NotifyCommand> configurator, [CallerMemberName]string name = null)
        {
            var c =  PropertyCache<TClass>.Get(name,
                        () => configurator(new NotifyConfigurator<TClass, NotifyCommand>())
                            .Compile());

            return new NotifyCommand(c);
        } 

        public static IObservableFilter<T> Filter<T>(NotifyConfiguratorFactory<TClass, ObservableFilter<T>> configurator, [CallerMemberName]string name = null)
        {
            var c =  PropertyCache<TClass>.Get(name,
                        () => configurator(new NotifyConfigurator<TClass, ObservableFilter<T>>().Init((target,filter)=>filter.OnTriggered()))
                            .Compile());

            return new ObservableFilter<T>(c);
        }

        private static NotifyActivator CreateActivatorA()
        {
            NotifyActivator activator = (t, p) => { };

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
                                activator += (t, p) =>
                                {
                                    var child = (IChildObject) propertyInfo.GetValue(t);
                                    child.SetParent(t, p);
                                };
                            }
                            else if (typeof(ICommand).IsAssignableFrom(propertyInfo.PropertyType))
                            {
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

            NotifyActivator activator = (a, b) => { };

            var type = typeof(TClass);
            //while (type != null)
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
                            activator += (o,p) =>
                            {
                                NotifyFactory.GetParser(o).GetTrigger(attribute.Path, (s, args) => (property.GetValue(o) as ITriggerable)?.OnTriggered());
                            };
                        }
                        else
                        {
                            activator += (o,p) =>
                            {
                                NotifyFactory.GetParser(o).GetTrigger(attribute.Path, (s, args) => p.OnPropertyChanged(new PropertyChangedEventArgs(property.Name)));
                            };
                        }
                    }
                }

                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance))
                {
                    foreach (var attribute in method.GetCustomAttributes().OfType<TriggerOnAttribute>())
                    {
                        activator += (o,p) =>
                        {
                            NotifyFactory.GetParser(o).GetTrigger(attribute.Path, (s, args) => method.Invoke(o, new object[] { }));
                        };
                    }
                }

                type = type.BaseType;
            }

            activator += (o,p) =>
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