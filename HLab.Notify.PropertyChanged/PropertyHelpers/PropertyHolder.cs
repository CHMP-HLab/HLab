using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace HLab.Notify.PropertyChanged
{
    public interface IPropertyHolderN
    {
        object Value{ get; set; }
        //bool Enabled { get; set; }
        //bool Mandatory { get; set; }
    }

    enum PropertyState
    {
        Enabled,
        Locked,
    }
    public interface IPropertyHolderN<T> : IPropertyHolderN
    {
        new T  Value{ get; set; }
    }

    public class PropertyHolderN<TClass, T> : PropertyHolder<TClass, T>, IPropertyHolderN<T>, INotifyPropertyChanged
        where TClass : NotifierBase
    {
        class H : NotifyHelper<PropertyHolderN<TClass, T>> { }
        public PropertyHolderN(string name, NotifyConfiguratorFactory<TClass, PropertyHolder<TClass,T>> configurator = null) : base(name, configurator)
        {
        }



        public T Value
        {
            get => _value.Get();
            set => _value.Set(value);
        }
        private readonly IProperty<T> _value = H.Property<T>();

        public bool Enabled
        {
            get => _enabled.Get();
            set => _enabled.Set(value);
        }
        object IPropertyHolderN.Value { get => Value; set => Value = (T)value; }

        private readonly IProperty<bool> _enabled = H.Property<bool>();
        public event PropertyChangedEventHandler PropertyChanged;


    }
    public class PropertyHolder<TClass, T> : PropertyHolder<T>
        where TClass : class
    {
        protected new PropertyCache<TClass>.ConfiguratorEntry<PropertyHolder<TClass,T>> Configurator
        {
            get => (PropertyCache<TClass>.ConfiguratorEntry<PropertyHolder<TClass,T>>)base.Configurator;
            set => base.Configurator = value;
        }

        protected override void Configure()
        {
            Configurator.Configure(Parent,Parser, this);

            if(PropertyValue==null)
                SetProperty(new PropertyValueLazy<T>(this, o => default(T)));
        }

        public PropertyHolder(string name,
            NotifyConfiguratorFactory<TClass, PropertyHolder<TClass,T>> configurator = null)
        {
            if (configurator != null)
                Configurator =
                    PropertyCache<TClass>.Get(name,
                        () => configurator(new NotifyConfigurator<TClass, PropertyHolder<TClass, T>>())
                            .Compile()
                        );
        }


        public new TClass Parent => base.Parent as TClass;

    }

    public abstract class PropertyHolder<T> : ChildObject, IProperty<T>
    {
        protected internal IPropertyValue<T> PropertyValue;
        public T Get() => PropertyValue.Get();

        public T Get([CallerMemberName] string name = null)
        {
#if DEBUG
            if (name != null && name != "SetOneToMany" && name != "Set")
                Debug.Assert(name == this.Name);

            if (PropertyValue == null) throw new Exception("Triggers not registered");
#endif
            return PropertyValue.Get();
        }

        public bool Set(T value)
        {
            if (PropertyValue == null) throw new Exception("Triggers not registered");
            return PropertyValue.Set(value);
        }

        public void SetProperty(IPropertyValue<T> property)
        {
            Interlocked.Exchange(ref PropertyValue, property);
        }
    }
}