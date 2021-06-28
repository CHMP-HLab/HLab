using System;
using System.Windows;
using System.Windows.Data;

namespace HLab.Base.Wpf
{
    public class DependencyPropertyConfigurator<TClass,TValue>
        where TClass : DependencyObject
    {
        private readonly string _name;
        private readonly FrameworkPropertyMetadata _propertyMetadata = new();
        private ValidateValueCallback _validateValueCallback = null;
        //private readonly TValue _default = default;
        public DependencyPropertyConfigurator(string name)
        {
            _name = name;
        }

        public DependencyProperty Register() => DependencyProperty.Register(
            _name,
            typeof(TValue),
            typeof(TClass),
            _propertyMetadata,
            _validateValueCallback
        );
        public DependencyProperty RegisterAttached<T>() => DependencyProperty.RegisterAttached(
            _name,
            typeof(TValue),
            typeof(T),
            _propertyMetadata,
            _validateValueCallback
        );

        public DependencyProperty RegisterAttached() => RegisterAttached<TClass>();

        public DependencyPropertyKey RegisterReadOnly() => DependencyProperty.RegisterReadOnly(
            _name,
            typeof(TValue),
            typeof(TClass),
            _propertyMetadata,
            _validateValueCallback
        );
        public DependencyPropertyKey RegisterAttachedReadOnly() => DependencyProperty.RegisterAttachedReadOnly(
            _name,
            typeof(TValue),
            typeof(TClass),
            _propertyMetadata,
            _validateValueCallback
        );

        private DependencyPropertyConfigurator<TClass,TValue> Do(Action action)
        {
            action();
            return this;
        }


        public DependencyPropertyConfigurator<TClass, TValue> AffectsMeasure => Do(() => _propertyMetadata.AffectsMeasure = true);
        public DependencyPropertyConfigurator<TClass, TValue> AffectsArrange => Do(() => _propertyMetadata.AffectsArrange = true);
        public DependencyPropertyConfigurator<TClass, TValue> AffectsParentMeasure => Do(() => _propertyMetadata.AffectsParentMeasure = true);
        public DependencyPropertyConfigurator<TClass, TValue> AffectsParentArrange => Do(() => _propertyMetadata.AffectsParentArrange = true);
        public DependencyPropertyConfigurator<TClass, TValue> AffectsRender => Do(() => _propertyMetadata.AffectsRender = true);
        public DependencyPropertyConfigurator<TClass, TValue> Inherits => Do(() => _propertyMetadata.Inherits = true);
        public DependencyPropertyConfigurator<TClass, TValue> OverridesInheritanceBehavior => Do(() => _propertyMetadata.OverridesInheritanceBehavior = true);
        public DependencyPropertyConfigurator<TClass, TValue> NotDataBindable => Do(() => _propertyMetadata.IsNotDataBindable = true);
        public DependencyPropertyConfigurator<TClass, TValue> BindsTwoWayByDefault => Do(() => _propertyMetadata.BindsTwoWayByDefault = true);
        public DependencyPropertyConfigurator<TClass, TValue> Journal => Do(() => _propertyMetadata.Journal = true);
        public DependencyPropertyConfigurator<TClass, TValue> SubPropertiesDoNotAffectRender => Do(() => _propertyMetadata.SubPropertiesDoNotAffectRender = true);
        public DependencyPropertyConfigurator<TClass, TValue> DefaultUpdateSourceTrigger(UpdateSourceTrigger trigger)  => Do(() => _propertyMetadata.DefaultUpdateSourceTrigger  = trigger);

        public DependencyPropertyConfigurator<TClass,TValue> OnChange<TSender>(Action<TSender, DependencyPropertyChangedEventArgs<TValue>> action)
            where TSender : DependencyObject
        {
            _propertyMetadata.PropertyChangedCallback += (d, e) =>
            {
                if (d is not TSender c) return;

                action(c, new DependencyPropertyChangedEventArgs<TValue>(e));
            };

            return this;
        }
        public DependencyPropertyConfigurator<TClass,TValue> OnChange<TSender>(Action<TSender> action)
            where TSender : DependencyObject
        {
            _propertyMetadata.PropertyChangedCallback += (d, e) =>
            {
                if (d is TSender c)
                {
                    action(c);
                }
            };

            return this;
        }

        public DependencyPropertyConfigurator<TClass, TValue> OnChange(Action<TClass, DependencyPropertyChangedEventArgs<TValue>> action)
            => OnChange<TClass>(action);
        public DependencyPropertyConfigurator<TClass, TValue> OnChange(Action<TClass> action)
            => OnChange<TClass>(action);

        public DependencyPropertyConfigurator<TClass,TValue> Validate(Func<TValue,bool> func)
        {
            _validateValueCallback += o => func((TValue)o);

            return this;
        }

        public DependencyPropertyConfigurator<TClass,TValue> Default(TValue value)
        {
            _propertyMetadata.DefaultValue = value;
            return this;
        }

    }
}