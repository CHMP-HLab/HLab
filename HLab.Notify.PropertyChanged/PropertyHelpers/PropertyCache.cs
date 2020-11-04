using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;

namespace HLab.Notify.PropertyChanged.PropertyHelpers
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

            public abstract void Configure(INotifyPropertyChangedWithHelper target, object member);
            //public Action<object, INotifyClassParser, object> Action { get; set; }
            public abstract void Update(object target, object member);

            public List<string> Dependencies { get; set; }

            public string HolderName { get; set; }
        }
        

    /// <summary>
    /// Hold 
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    public static class PropertyCache<TClass>
        where TClass : class, INotifyPropertyChangedWithHelper
    {

        public class ConfiguratorEntry<T> : ConfiguratorEntry
        {
            public Action<TClass, T> Action { get; set; }
            public Action<TClass, T> UpdateAction { get; set; }

            public void Configure(TClass target, T member)
            {
                Action?.Invoke(target, member);
            }
            public void Update(TClass target, T member)
            {
                UpdateAction?.Invoke(target, member);
            }

            public override void Configure(INotifyPropertyChangedWithHelper target, object member) =>
                Configure((TClass)target,(T)member);
            public override void Update(object target, object member) =>
                Update((TClass)target, (T)member);
        }

        private static readonly ConcurrentDictionary<string, ConfiguratorEntry> CacheByHolder = new ConcurrentDictionary<string, ConfiguratorEntry>();
        private static readonly ConcurrentDictionary<string, ConfiguratorEntry> CacheByProperty = new ConcurrentDictionary<string, ConfiguratorEntry>();
        public static ConfiguratorEntry GetByProperty(string name )
        {
            if(CacheByProperty.TryGetValue(name, out var e)) return e;

            throw new Exception("Error");

        }
        public static ConfiguratorEntry GetByHolder(string name )
        {
            if(CacheByHolder.TryGetValue(name, out var e)) return e;

            throw new ArgumentException($"PropertyHolder {name} not found in {typeof(TClass)}");

        }

        public static ConfiguratorEntry<T> GetByHolder<T>(string name,
            Func<string,NotifyConfigurator<TClass,T>.Activator> activator)
        {
            return (ConfiguratorEntry<T>)CacheByHolder.GetOrAdd(name,valueFactory: n =>
            {
                var a = activator(n);

                return (ConfiguratorEntry<T>)CacheByProperty.GetOrAdd(a.PropertyName,valueFactory: n2 => new ConfiguratorEntry<T>
                {
                    EventArgs = new PropertyChangedEventArgs(a.PropertyName), 
                    HolderName = n2, 
                    Dependencies = a.DependsOn,
                    Action = a.Action,
                    UpdateAction = a.UpdateAction
                });
            });
        }
    }

}




