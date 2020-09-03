using System;
using System.Runtime.CompilerServices;
using System.Windows;

namespace HLab.Base
{
    public class ChangedEventArg<TValue>
    {
        public TValue OldValue { get; internal set; }
        public TValue NewValue { get; internal set; }

        public DependencyProperty Property { get; internal set; }
    }



    public class DependencyConfigurator<TClass,TValue>
        where TClass : DependencyObject
    {
        private readonly string _name;
        private readonly FrameworkPropertyMetadata _propertyMetadata = new FrameworkPropertyMetadata();
        private ValidateValueCallback _validateValueCallback = null;
        private TValue _default = default(TValue);
        public DependencyConfigurator(string name)
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

        private DependencyConfigurator<TClass,TValue> Do(Action action)
        {
            action();
            return this;
        }


        public DependencyConfigurator<TClass, TValue> AffectsMeasure => Do(() => _propertyMetadata.AffectsMeasure = true);
        public DependencyConfigurator<TClass, TValue> AffectsArrange => Do(() => _propertyMetadata.AffectsArrange = true);
        public DependencyConfigurator<TClass, TValue> AffectsParentMeasure => Do(() => _propertyMetadata.AffectsParentMeasure = true);
        public DependencyConfigurator<TClass, TValue> AffectsParentArrange => Do(() => _propertyMetadata.AffectsParentArrange = true);
        public DependencyConfigurator<TClass, TValue> AffectsRender => Do(() => _propertyMetadata.AffectsRender = true);
        public DependencyConfigurator<TClass, TValue> Inherits => Do(() => _propertyMetadata.Inherits = true);
        public DependencyConfigurator<TClass, TValue> OverridesInheritanceBehavior => Do(() => _propertyMetadata.OverridesInheritanceBehavior = true);
        public DependencyConfigurator<TClass, TValue> NotDataBindable => Do(() => _propertyMetadata.IsNotDataBindable = true);
        public DependencyConfigurator<TClass, TValue> BindsTwoWayByDefault => Do(() => _propertyMetadata.BindsTwoWayByDefault = true);
        public DependencyConfigurator<TClass, TValue> Journal => Do(() => _propertyMetadata.Journal = true);
        public DependencyConfigurator<TClass, TValue> SubPropertiesDoNotAffectRender => Do(() => _propertyMetadata.SubPropertiesDoNotAffectRender = true);

        public DependencyConfigurator<TClass,TValue> OnChange<TSender>(Action<TSender, ChangedEventArg<TValue>> action)
        where TSender : DependencyObject
        {
            _propertyMetadata.PropertyChangedCallback += (d, e) =>
            {
                if (d is TSender c)
                {
                    var newValue = (TValue)e.NewValue;
                    var oldValue = (TValue)e.OldValue;

                    action(c,
                        new ChangedEventArg<TValue>
                        { Property = e.Property, NewValue = newValue, OldValue = oldValue });
                }
            };

            return this;
        }
        public DependencyConfigurator<TClass,TValue> OnChange<TSender>(Action<TSender> action)
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

        public DependencyConfigurator<TClass, TValue> OnChange(Action<TClass, ChangedEventArg<TValue>> action)
            => OnChange<TClass>(action);
        public DependencyConfigurator<TClass, TValue> OnChange(Action<TClass> action)
            => OnChange<TClass>(action);

        public DependencyConfigurator<TClass,TValue> Validate(Func<TValue,bool> func)
       {
           _validateValueCallback += o => func((TValue)o);

            return this;
        }

        public DependencyConfigurator<TClass,TValue> Default(TValue value)
        {
            _propertyMetadata.DefaultValue = _default;
            return this;
        }

    }

    public class DependencyHelper
    {
        public static DependencyConfigurator<TClass, TValue> Property<TClass, TValue>([CallerMemberName] string name = null)
            where TClass : DependencyObject
        {
            if (name == null) throw new NullReferenceException();

            if (name.EndsWith("Property")) name = name.Substring(0, name.Length - 8);
            return new DependencyConfigurator<TClass, TValue>(name);
        }
    }


    public class DependencyHelper<TClass> : DependencyHelper
    where TClass : DependencyObject
    {
        public static DependencyConfigurator<TClass, TValue> Property<TValue>([CallerMemberName] string name = null)
            => Property<TClass, TValue>(name);
    }
}
