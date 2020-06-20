using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace HLab.Notify.PropertyChanged
{
//    public abstract class PropertyHolder<T> : PropertyHolder, IProperty<T>
//    {
//        protected internal IPropertyValue<T> PropertyValue;
//        public T Get() => PropertyValue.Get();

//        public T Get([CallerMemberName] string name = null)
//        {
//#if DEBUG
//            if (name != null && name != "SetOneToMany" && name != "Set")
//                Debug.Assert(name == this.Name);

//            if (PropertyValue == null) throw new Exception("Triggers not registered");
//#endif
//            return PropertyValue.Get();
//        }

//        public bool Set(T value)
//        {
//            if (PropertyValue == null) throw new Exception("Triggers not registered");
//            return PropertyValue.Set(value);
//        }

//        public void SetProperty(IPropertyValue<T> property)
//        {
//            Interlocked.Exchange(ref PropertyValue, property);
//        }
//    }

    //public class PropertyHolder<TClass, T> : PropertyHolder<T>
    //where TClass : NotifierBase
    //{
    //    protected new PropertyConfigurator<TClass, T> Configurator
    //    {
    //        get => (PropertyConfigurator<TClass, T>)base.Configurator;
    //        set => base.Configurator = value;
    //    }

    //    public override void SetParent(object parent)
    //    {
    //        base.SetParent(parent);

    //        Configurator?.Register?.Invoke(Parent, this);


    //        SetProperty(new PropertyValueLazy<T>(this, n => Configurator.DefaultSetter((TClass)n)));
    //    }

    //    public PropertyHolder(string name,
    //        Func<PropertyConfigurator<TClass, T>, PropertyConfigurator<TClass, T>> configurator = null)
    //    {
    //        if (configurator != null)
    //            Configurator =
    //                PropertyCache<TClass>.Get(name,
    //                    n => configurator(new PropertyConfigurator<TClass, T>(name)).Compile());
    //    }

    //    public new TClass Parent => base.Parent as TClass;

    //}


        public abstract class ConfiguratorEntry
        {
            #if DEBUG
            public object Source{ get;internal set; }
            #endif
            public PropertyChangedEventArgs EventArgs { get; internal set; }

            public abstract void Configure(object target, INotifyClassParser parser, object member);
        }
        

    /// <summary>
    /// Hold 
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    public static class PropertyCache<TClass>
        where TClass : class
    {

        public class ConfiguratorEntry<T> : ConfiguratorEntry
        {
            private Action<TClass, INotifyClassParser, T> _configure;
            public void SetAction(Action<TClass, INotifyClassParser, T> action) => _configure = action;

            public void Configure(TClass target, INotifyClassParser parser, T member)
            {
                _configure?.Invoke(target, parser, member);
            }

            public override void Configure(object target, INotifyClassParser parser, object member) =>
                Configure((TClass) target, parser, (T) member);
        }

        private static readonly ConcurrentDictionary<string, ConfiguratorEntry>
            // ReSharper disable once StaticMemberInGenericType
            _cache = new ConcurrentDictionary<string, ConfiguratorEntry>();

        public static ConfiguratorEntry<T> Get<T>(string name,
            Func<Action<TClass,INotifyClassParser,T>> action)
        {
            return (ConfiguratorEntry<T>)_cache.GetOrAdd(name,valueFactory: n =>
            {
                var entry = new ConfiguratorEntry<T> {EventArgs = new PropertyChangedEventArgs(name)};
                entry.SetAction(action());

                return entry;
            });
        }
    }

}




