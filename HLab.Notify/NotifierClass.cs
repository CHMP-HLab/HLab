using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using HLab.DependencyInjection.Annotations;
using HLab.Notify.Annotations;

namespace HLab.Notify
{
    public interface INotifierClass
    {
        Type ClassType { get; }
        void Subscribe(INotifier n);
    }

    public interface INotifierClass<TClass> : INotifierClass
    {
        INotifierProperty<T> GetProperty<T>(PropertyInfo property);
        INotifierProperty<T> GetProperty<T>(string name);
        INotifierProperty GetProperty(string name);

    }

    [Export(typeof(INotifierClass<>)),GenericAsTarget]
    public class NotifierClass<TClass> : INotifierClass<TClass>, IInitializer
    {
        public NotifierClass()
        {
            Register();
        }

        public override string ToString() => ClassType?.Name;
        public void Initialize(IRuntimeImportContext ctx, object[] args)
        {
        }

        public Type ClassType => typeof(TClass);

        private readonly ConcurrentDictionary<string, INotifierProperty> _propertiesByName =
            new ConcurrentDictionary<string, INotifierProperty>();

        private readonly ConcurrentDictionary<PropertyInfo, INotifierProperty> _properties =
            new ConcurrentDictionary<PropertyInfo, INotifierProperty>();

        public INotifierProperty<T> GetProperty<T>(PropertyInfo property)
        {
            var notifierProperty = _properties.GetOrAdd(property, GetNewProperty<T>);
            return  (INotifierProperty<T>)notifierProperty;
        }

        private static readonly MethodInfo GetNewPropertyInfo = typeof(NotifierClass<TClass>).GetMethod("GetNewProperty", BindingFlags.NonPublic | BindingFlags.Instance);

        public INotifierProperty GetProperty(PropertyInfo property)
        {
            return _properties.GetOrAdd(property, (pi) =>
            {
                var method = GetNewPropertyInfo.MakeGenericMethod(pi.PropertyType);
                return (INotifierProperty) method.Invoke(this, new object[] {pi});
            });
        }

        public INotifierProperty GetProperty(string propertyName)
        {
            return _propertiesByName.GetOrAdd(propertyName, n =>
            {
                var property = typeof(TClass).GetProperty(n, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if(property==null)
                { }

                return GetProperty(property);
            });
        }

        public INotifierProperty<T> GetProperty<T>(string name)
        {
            var p1 = _propertiesByName.GetOrAdd(name, n =>
            {
                //if name start whith # do not check
                if (n[0] == '#')
                {
                    n = n.TrimStart('#');
                    var p = ClassType.GetProperty(n);
                    return p == null ? GetNewPropertyByName<T>(n) : GetProperty<T>(p);
                }

                var property = ClassType.GetProperty(n);
                if (property == null)
                {
                    throw new PropertyNotFoundException("Property " + n + " not found on " + ClassType.FullName);
                }

                return GetProperty(property);
            });

            if(p1 is INotifierProperty<T> pp) return pp;
            var message = (p1.GetType().GenericReadableName() +  "not castable to " + typeof(INotifierProperty<T>).GenericReadableName());
            throw new Exception(message);
        }


        [Import] private Func<Type,object[],object> _locate;
        private T Locate<T>(params object[] args) => (T)_locate(typeof(T),args);
        protected INotifierProperty<T> GetNewPropertyByName<T>(string name) => Locate<NotifierProperty<TClass,T>>(new object[]{this, name});
        protected INotifierPropertyReflexion<T> GetNewProperty<T>(PropertyInfo property)
        {
            return Locate<NotifierPropertyReflexion<TClass, T>>(this, property );
        }


        public void Subscribe(INotifier n)
        {
            _subscribe(n);
        }

        private Action<INotifier> _subscribe;
        private void Register()
        {
            Action<INotifier> action = n => {};


            //Register all methods
            foreach (var method in ClassType
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                Func<object, PropertyChangedEventHandler> getHandler = null;

                var prm = method.GetParameters();

                switch (prm.Length)
                {
                    case 0:
                        getHandler = target => (s, args) => method.Invoke(target, null);
                        break;
                    case 1:
                        if (
                            prm[0].ParameterType.IsAssignableFrom(typeof(NotifierPropertyChangedEventArgs))
                        )
                        getHandler = target => (s, args) => method.Invoke(target, new object[] { args });
                        break;
                    case 2:
                        if(
                            prm[0].ParameterType.IsAssignableFrom(typeof(object)) &&
                            prm[1].ParameterType.IsAssignableFrom(typeof(NotifierPropertyChangedEventArgs))                            
                            )
                            getHandler = target => (s, args) => method.Invoke(target, new[] { s, args });
                        break;
                }

                foreach (var triggerOn in method.GetCustomAttributes().OfType<TriggerOnAttribute>())
                {
                    Debug.Assert(getHandler != null, ClassType.Name + "." + method.Name +  " : Wrong method signature");

                    //if no path method get executed at subscribing
                    if (triggerOn.Path == null)
                    {
                        action += n => getHandler(n.Target).Invoke(n.Target, new NotifierPropertyChangedEventArgs("",null,null));                        
                    }
                    else
                    {
                        //var signalProperty = this[triggerOn.Path.PropertyName];
                        //var path = triggerOn.Path.Next;

                        //action += n => n[signalProperty].Subscribe(getHandler(n.Target), path);
                        try
                        {
                            var signalProperty = GetProperty(triggerOn.Path.PropertyName);
                            var path = triggerOn.Path.Next;
                            action += n =>
                            {
                                n.GetPropertyEntry(signalProperty).Subscribe(getHandler(n.Target), path);
                            };
                        }
                        catch (PropertyNotFoundException e)
                        {
                            var m = "TriggerOn Error\nClass : " + ClassType.Name + "\nProperty : " +
                                    method.Name;
                            throw new PropertyNotFoundException(m + "\n" + e.Message, e);
                        }
                    }
                }
            }

            //Register all properties
            foreach (var property in ClassType.GetProperties(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {

                var slotProperty = GetProperty(property);

                foreach (var triggerOn in property.GetCustomAttributes().OfType<TriggerOnAttribute>())
                {
                    if (triggerOn.Path == null)
                    {
                        action += n =>
                        {
                            n.GetPropertyEntry(slotProperty).Update();
                        };
                    }
                    else
                    {
                        try
                        {
                            var signalProperty = GetProperty(triggerOn.Path.PropertyName);
                            var path = triggerOn.Path.Next;
                            action += n =>
                            {
                                    n.GetPropertyEntry(signalProperty).Subscribe(n.GetPropertyEntry(slotProperty).OnTrigger, path);
                            };
                        }
                        catch (PropertyNotFoundException e)
                        {
                            var m = "TriggerOn Error\nClass : " + ClassType.Name + "\nProperty : " +
                                       property.Name;
                            throw new PropertyNotFoundException(m + "\n" + e.Message, e);
                        }
                    }
                }
            }

            _subscribe = action;
        }
    }

    public class PropertyNotFoundException : Exception
    {
        public PropertyNotFoundException(string message,Exception e):base(message,e)
        { }
        public PropertyNotFoundException(string message):base(message)
        { }
    }
}