﻿using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged.NotifyHelpers;

namespace HLab.Notify.PropertyChanged.PropertyHelpers
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

    public class PropertyHolderN<TClass, T> : PropertyHolder<T>, IPropertyHolderN<T>, INotifyPropertyChangedWithHelper
        where TClass : NotifierBase
    {
        class H : H<PropertyHolderN<TClass, T>> { }
        public PropertyHolderN(PropertyActivator activator = null) : base(activator)
        {
            ClassHelper = new NotifyClassHelper(this);
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


        public INotifyClassHelper ClassHelper { get; }
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
        protected PropertyHolder(PropertyActivator activator) : base(activator)
        {
        }
    }


    public class PropertyHolder<T> : ChildObject, IProperty<T>
    {
        protected internal IPropertyValue<T> PropertyValue;

        public PropertyHolder(PropertyActivator activator) : base(activator)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get()
        {
            // Todo : necessary for overloaded
            if (PropertyValue == null) return default;
            return PropertyValue.Get();
        }

#if DEBUG
        [DebuggerStepThrough]
        public T Get([CallerMemberName] string name = null)
        {
            if (name != null && name != "SetOneToMany" && name != "Set")
                Debug.Assert(name == this.Name);

            if (PropertyValue == null) return default;
            //throw new Exception("Triggers not registered");

            return PropertyValue.Get();
        }
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Set(T value)
        {
            Debug.Assert(PropertyValue != null);
            return PropertyValue.Set(value);
        }

        public void SetProperty(IPropertyValue<T> property)
        {
            Interlocked.Exchange(ref PropertyValue, property);
        }
        protected override void Activate()
        {
            base.Activate();

            if(PropertyValue==null)
                SetProperty(new PropertyValueLazy<T>(this, o => default(T)));
        }
    }
}