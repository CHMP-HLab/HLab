using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using HLab.Notify.PropertyChanged.PropertyHelpers;

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

    public class PropertyHolderN<TClass, T> : PropertyHolder<T>, IPropertyHolderN<T>, INotifyPropertyChanged
        where TClass : NotifierBase
    {
        class H : NotifyHelper<PropertyHolderN<TClass, T>> { }
        public PropertyHolderN(ConfiguratorEntry configurator = null) : base(configurator)
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
    //public class PropertyHolder<TClass, T> : PropertyHolder<T>
    //    where TClass : class
    //{


    //    public PropertyHolder(string name, NotifyConfigurator configurator):base(configurator)
    //    {
    //    }


    //    public new TClass Parent => base.Parent as TClass;

    //}
    public abstract class PropertyHolder : ChildObject
    {
        protected PropertyHolder(ConfiguratorEntry configurator) : base(configurator)
        {
        }
    }


    public class PropertyHolder<T> : ChildObject, IProperty<T>
    {
        protected internal IPropertyValue<T> PropertyValue;

        public PropertyHolder(ConfiguratorEntry configurator) : base(configurator)
        {
        }

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
        protected override void Configure()
        {
            base.Configure();

            if(PropertyValue==null)
                SetProperty(new PropertyValueLazy<T>(this, o => default(T)));
        }
    }
}