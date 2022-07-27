using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace HLab.Base.Wpf
{
    public class DependencyPropertyConfigurator<TClass,TValue>
        where TClass : DependencyObject
    {
        readonly string _name;
        readonly FrameworkPropertyMetadata _propertyMetadata = new();

        ValidateValueCallback _validateValueCallback = null;
        //private readonly TValue _default = default;
        public DependencyPropertyConfigurator(string name)
        {
            _name = name;
        }

        /// <summary>
        ///    Registers a dependency property with the specified property name, property type,
        ///    owner type, property metadata, and a value validation callback for the property.
        /// </summary>
        /// <returns></returns>
        public DependencyProperty Register() => DependencyProperty.Register(
            _name,
            typeof(TValue),
            typeof(TClass),
            _propertyMetadata,
            _validateValueCallback
        );

        /// <summary>
        ///     Registers an attached property with the specified property type, owner type,
        ///     property metadata, and value validation callback for the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DependencyProperty RegisterAttached<T>() => DependencyProperty.RegisterAttached(
            _name,
            typeof(TValue),
            typeof(T),
            _propertyMetadata,
            _validateValueCallback
        );

        /// <summary>
        ///     Registers an attached property with the specified property type, owner type,
        ///     property metadata, and value validation callback for the property.
        /// </summary>
        /// <returns></returns>
        public DependencyProperty RegisterAttached() => RegisterAttached<TClass>();

        /// <summary>
        ///     Registers a read-only dependency property, with the specified property type,
        ///     owner type, property metadata, and a validation callback.
        /// </summary>
        /// <returns></returns>
        public DependencyPropertyKey RegisterReadOnly() => DependencyProperty.RegisterReadOnly(
            _name,
            typeof(TValue),
            typeof(TClass),
            _propertyMetadata,
            _validateValueCallback
        );

        /// <summary>
        ///     Registers a read-only attached property, with the specified property type, owner
        ///     type, property metadata, and a validation callback.
        /// </summary>
        /// <returns></returns>
        public DependencyPropertyKey RegisterAttachedReadOnly() => DependencyProperty.RegisterAttachedReadOnly(
            _name,
            typeof(TValue),
            typeof(TClass),
            _propertyMetadata,
            _validateValueCallback
        );

        DependencyPropertyConfigurator<TClass,TValue> Do(Action action)
        {
            action();
            return this;
        }

        /// <summary>
        ///     Gets or sets a value that indicates whether a dependency property potentially
        ///     affects the measure pass during layout engine operations.
        /// </summary>
        public DependencyPropertyConfigurator<TClass, TValue> AffectsMeasure => Do(() => _propertyMetadata.AffectsMeasure = true);

        /// <summary>
        ///     Gets or sets a value that indicates whether a dependency property potentially
        ///     affects the arrange pass during layout engine operations.
        /// </summary>
        public DependencyPropertyConfigurator<TClass, TValue> AffectsArrange => Do(() => _propertyMetadata.AffectsArrange = true);

        /// <summary>
        ///     Gets or sets a value that indicates whether a dependency property potentially
        ///     affects the measure pass of its parent element's layout during layout engine
        ///     operations.
        /// </summary>
        public DependencyPropertyConfigurator<TClass, TValue> AffectsParentMeasure => Do(() => _propertyMetadata.AffectsParentMeasure = true);

        /// <summary>
        ///     Dependency property potentially
        ///     affects the arrange pass of its parent element's layout during layout engine
        ///     operations.
        /// </summary>
        public DependencyPropertyConfigurator<TClass, TValue> AffectsParentArrange => Do(() => _propertyMetadata.AffectsParentArrange = true);

        /// <summary>
        ///     Gets or sets a value that indicates whether a dependency property potentially
        ///     affects the general layout in some way that does not specifically influence arrangement
        ///     or measurement, but would require a redraw.
        /// </summary>
        public DependencyPropertyConfigurator<TClass, TValue> AffectsRender => Do(() => _propertyMetadata.AffectsRender = true);

        /// <summary>
        ///     The dependency property is inheritable.
        /// </summary>
        public DependencyPropertyConfigurator<TClass, TValue> Inherits => Do(() => _propertyMetadata.Inherits = true);

        /// <summary>
        ///     Property value inheritance evaluation should span across certain content boundaries in the logical tree of elements.
        /// </summary>
        public DependencyPropertyConfigurator<TClass, TValue> OverridesInheritanceBehavior => Do(() => _propertyMetadata.OverridesInheritanceBehavior = true);

        /// <summary>
        ///     The dependency property does not support data binding.
        /// </summary>
        public DependencyPropertyConfigurator<TClass, TValue> NotDataBindable => Do(() => _propertyMetadata.IsNotDataBindable = true);

        /// <summary>
        ///     The property binds two-way by default.
        /// </summary>
        public DependencyPropertyConfigurator<TClass, TValue> BindsTwoWayByDefault => Do(() => _propertyMetadata.BindsTwoWayByDefault = true);

        /// <summary>
        ///     This property contains journaling information that applications can or should store as part of a journaling implementation.
        /// </summary>
        public DependencyPropertyConfigurator<TClass, TValue> Journal => Do(() => _propertyMetadata.Journal = true);

        /// <summary>
        ///     Sub-properties of the dependency property do not affect the rendering of the containing object.
        /// </summary>
        public DependencyPropertyConfigurator<TClass, TValue> SubPropertiesDoNotAffectRender => Do(() => _propertyMetadata.SubPropertiesDoNotAffectRender = true);

        /// <summary>
        ///     Gets or sets the default for System.Windows.Data.UpdateSourceTrigger to use when
        ///     bindings for the property with this metadata are applied, which have their System.Windows.Data.UpdateSourceTrigger
        ///     set to System.Windows.Data.UpdateSourceTrigger.Default.
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public DependencyPropertyConfigurator<TClass, TValue> DefaultUpdateSourceTrigger(UpdateSourceTrigger trigger)  => Do(() => _propertyMetadata.DefaultUpdateSourceTrigger  = trigger);

        /// <summary>
        ///     Represents the callback that is invoked when the effective property value of
        ///     a dependency property changes.
        /// </summary>
        /// <typeparam name="TSender"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public DependencyPropertyConfigurator<TClass,TValue> OnChange<TSender>(Action<TSender, DependencyPropertyChangedEventArgs<TValue>> action)
            where TSender : DependencyObject
        {
            _propertyMetadata.PropertyChangedCallback += (sender, args) =>
            {
                if (sender is TSender typedSender)
                {
                    action(typedSender, new DependencyPropertyChangedEventArgs<TValue>(args));
                }
                else
                {
                    if(sender==null)
                    {
                        action(null, new DependencyPropertyChangedEventArgs<TValue>(args));
                    }
                }
            };
            return this;
        }

        public DependencyPropertyConfigurator<TClass,TValue> OnChange<TSender>(Func<TSender, DependencyPropertyChangedEventArgs<TValue>, Task> action)
            where TSender : DependencyObject
        {

            _propertyMetadata.PropertyChangedCallback += async (sender, args) =>
            {
                switch (sender)
                {
                    case TSender typedSender:
                        //await typedSender.Dispatcher.InvokeAsync(async () =>
                            await action(typedSender, new DependencyPropertyChangedEventArgs<TValue>(args))
                            //)
                            ;

                        break;
                    
                    case null:
                        await action(null, new DependencyPropertyChangedEventArgs<TValue>(args));
                        break;
                }
            };
            return this;
        }

        /// <summary>
        ///     Represents the callback that is invoked when the effective property value of
        ///     a dependency property changes.
        /// </summary>
        /// <typeparam name="TSender"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public DependencyPropertyConfigurator<TClass,TValue> OnChange<TSender>(Action<TSender> action)
            where TSender : DependencyObject
        {
            _propertyMetadata.PropertyChangedCallback += (sender, args) =>
            {
                if (sender is TSender typedSender)
                {
                    action(typedSender);
                }
            };

            return this;
        }


        /// <summary>
        ///     Represents the callback that is invoked when the effective property value of
        ///     a dependency property changes.
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public DependencyPropertyConfigurator<TClass, TValue> OnChange(Action<TClass, DependencyPropertyChangedEventArgs<TValue>> action)
            => OnChange<TClass>(action);
        public DependencyPropertyConfigurator<TClass, TValue> OnChange(Func<TClass, DependencyPropertyChangedEventArgs<TValue>, Task> action)
            => OnChange<TClass>(action);

        /// <summary>
        ///     Represents the callback that is invoked when the effective property value of
        ///     a dependency property changes.
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public DependencyPropertyConfigurator<TClass, TValue> OnChange(Action<TClass> action)
            => OnChange<TClass>(action);

        /// <summary>
        ///     Represents a method used as a callback that validates the effective value of
        ///     a dependency property.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public DependencyPropertyConfigurator<TClass,TValue> Validate(Func<TValue,bool> func)
        {
            _validateValueCallback += o => func((TValue)o);

            return this;
        }

        /// <summary>
        ///     Sets the default value of the dependency property.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DependencyPropertyConfigurator<TClass,TValue> Default(TValue value)
        {
            _propertyMetadata.DefaultValue = value;
            return this;
        }

    }
}