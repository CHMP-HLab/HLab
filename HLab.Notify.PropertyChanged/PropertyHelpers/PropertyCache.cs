using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using HLab.Notify.Annotations;

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


        //public abstract class ConfiguratorEntry
        //{
        //    #if DEBUG
        //    public object Source{ get;internal set; }
        //    #endif
        //    public PropertyChangedEventArgs EventArgs { get; internal set; }

        //    public abstract void Configure(INotifyPropertyChangedWithHelper target, object member);
        //    //public Action<object, INotifyClassParser, object> Action { get; set; }
        //    public abstract void Update(object target, object member);

        //    public List<string> Dependencies { get; set; }

        //    public string HolderName { get; set; }
        //}
        




    /// <summary>
    /// Hold 
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    public static class PropertyCache<TClass>
        where TClass : class, INotifyPropertyChangedWithHelper
    {

        static PropertyCache()
        {
            var type = typeof(TClass).BaseType;
            if (type != null && typeof(INotifyPropertyChangedWithHelper).IsAssignableFrom(type))
            {
                var t = typeof(PropertyCache<>).MakeGenericType(type);
                var getByHolder = t.GetMethod("GetByHolder", new[] {typeof(string)});
                Debug.Assert(getByHolder!=null);
                _getByHolderParent = s => (PropertyActivator)getByHolder.Invoke(null,new object[]{s});
            }
        }

        //public class ConfiguratorEntry<T> : ConfiguratorEntry
        //{
        //    public Action<TClass, T> Action { get; set; }
        //    public Action<TClass, T> UpdateAction { get; set; }

        //    public void Configure(TClass target, T member)
        //    {
        //        Action?.Invoke(target, member);
        //    }
        //    public void Update(TClass target, T member)
        //    {
        //        UpdateAction?.Invoke(target, member);
        //    }

        //    public override void Configure(INotifyPropertyChangedWithHelper target, object member) =>
        //        Configure((TClass)target,(T)member);
        //    public override void Update(object target, object member) =>
        //        Update((TClass)target, (T)member);
        //}

        private static Func<string,PropertyActivator> _getByHolderParent;
        private static readonly ConcurrentDictionary<string, PropertyActivator> CacheByHolder = new();
        private static readonly ConcurrentDictionary<string, PropertyActivator> CacheByProperty = new();
        public static PropertyActivator GetByProperty(string name )
        {
            if(CacheByProperty.TryGetValue(name, out var e)) return e;
            throw new Exception("Error");
        }
        public static PropertyActivator GetByHolder(string name )
        {
            if(CacheByHolder.TryGetValue(name, out var e)) return e;

            if(_getByHolderParent == null) throw new ArgumentException($"PropertyHolder {name} not found in {typeof(TClass)}");
            return _getByHolderParent(name);

        }

        public static PropertyActivator GetByHolder<T>(string name,
            Func<string,NotifyConfigurator<TClass,T>.Activator> activator)
            where T : class,IChildObject
        {
            return CacheByHolder.GetOrAdd(name,valueFactory: n =>
            {
                var a = activator(n);

                return CacheByProperty.GetOrAdd(a.PropertyName,n2 => a);
            });
        }
    }

}




