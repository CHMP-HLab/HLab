using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;

namespace HLab.Base
{

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public readonly struct DependencyPropertyChangedEventArgs<TValue> 
  {
      /// <summary>Initializes a new instance of the <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> class.</summary>
      /// <param name="uncastedEventArgs"></param>
      public DependencyPropertyChangedEventArgs(
          DependencyPropertyChangedEventArgs uncastedEventArgs)
      {
          _uncastedEventArgs = uncastedEventArgs;
      }

      private readonly DependencyPropertyChangedEventArgs _uncastedEventArgs;

      /// <summary>Gets the value of the property after the change.</summary>
      /// <returns>The property value after the change.</returns>
      public TValue NewValue => (TValue) _uncastedEventArgs.NewValue;

      /// <summary>Gets the value of the property before the change.</summary>
      /// <returns>The property value before the change.</returns>
      public TValue OldValue => (TValue) _uncastedEventArgs.OldValue;

      /// <summary>Gets the identifier for the dependency property where the value change occurred.</summary>
      /// <returns>The identifier field of the dependency property where the value change occurred.</returns>
      public DependencyProperty Property => _uncastedEventArgs.Property;

    /// <summary>Determines whether the provided object is equivalent to the current <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />.</summary>
    /// <param name="obj">The object to compare to the current <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />.</param>
    /// <returns>
    /// <see langword="true" /> if the provided object is equivalent to the current <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />; otherwise, <see langword="false" />.</returns>
    public override bool Equals(object obj) => Equals((DependencyPropertyChangedEventArgs<TValue>) obj);

    /// <summary>Determines whether the provided <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> is equivalent to the current <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />.</summary>
    /// <param name="args">The <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> to compare to the current <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /></param>
    /// <returns>
    /// <see langword="true" /> if the provided <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> is equivalent to the current <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />; otherwise, <see langword="false" />.</returns>
    public bool Equals(DependencyPropertyChangedEventArgs<TValue> args) =>
        _uncastedEventArgs.Equals(args._uncastedEventArgs);

    /// <summary>Gets a hash code  for this <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />.</summary>
    /// <returns>A signed 32-bit integer hash code.</returns>
    public override int GetHashCode() => _uncastedEventArgs.GetHashCode();

    /// <summary>Determines whether two specified <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> objects have the same value.</summary>
    /// <param name="left">The first <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> to compare.</param>
    /// <param name="right">The second <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> to compare.</param>
    /// <returns>
    /// <see langword="true" /> if the two <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> instances are equivalent; otherwise, <see langword="false" />.</returns>
    public static bool operator ==(
        DependencyPropertyChangedEventArgs<TValue> left,
        DependencyPropertyChangedEventArgs<TValue> right) => left._uncastedEventArgs == right._uncastedEventArgs;

    /// <summary>Determines whether two specified <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> objects are different.</summary>
    /// <param name="left">The first <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> to compare.</param>
    /// <param name="right">The second <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> to compare.</param>
    /// <returns>
    /// <see langword="true" /> if the two <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> instances are different; otherwise, <see langword="false" />.</returns>
    public static bool operator !=(
      DependencyPropertyChangedEventArgs<TValue> left,
      DependencyPropertyChangedEventArgs<TValue> right) =>  left._uncastedEventArgs != right._uncastedEventArgs;
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
        public DependencyConfigurator<TClass, TValue> DefaultUpdateSourceTrigger(UpdateSourceTrigger trigger)  => Do(() => _propertyMetadata.DefaultUpdateSourceTrigger  = trigger);

        public DependencyConfigurator<TClass,TValue> OnChange<TSender>(Action<TSender, DependencyPropertyChangedEventArgs<TValue>> action)
        where TSender : DependencyObject
        {
            _propertyMetadata.PropertyChangedCallback += (d, e) =>
            {
                if (!(d is TSender c)) return;

                action(c, new DependencyPropertyChangedEventArgs<TValue>(e));
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

        public DependencyConfigurator<TClass, TValue> OnChange(Action<TClass, DependencyPropertyChangedEventArgs<TValue>> action)
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
