using Avalonia;
using Avalonia.Data;

namespace HLab.Base.Avalonia
{
    public class DependencyPropertyConfigurator<TClass,TValue>
        where TClass : IAvaloniaObject
    {
        readonly string _name;
        TValue _defaultValue;
        BindingMode _bindingMode;
        Func<TValue, bool>? _validate;
        Func<IAvaloniaObject, TValue, TValue>? _coerce;
        Action<IAvaloniaObject, bool>? _notifying;
        bool _inherits = false;
        //readonly FrameworkPropertyMetadata _propertyMetadata = new();

        //ValidateValueCallback _validateValueCallback = null;
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
        public StyledProperty<TValue> Register() => AvaloniaProperty.Register<TClass,TValue>(
            _name,_defaultValue,_inherits,_bindingMode,_validate,_coerce,_notifying
            //_propertyMetadata,
            //_validateValueCallback
        );

        /// <summary>
        ///     Registers an attached property with the specified property type, owner type,
        ///     property metadata, and value validation callback for the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public AttachedProperty<TValue> RegisterAttached<T>() => AvaloniaProperty.RegisterAttached<T,TClass,TValue>(
            _name,_defaultValue,_inherits, _bindingMode,_validate,_coerce
            //_propertyMetadata,
            //_validateValueCallback
        );

        /// <summary>
        ///     Registers an attached property with the specified property type, owner type,
        ///     property metadata, and value validation callback for the property.
        /// </summary>
        /// <returns></returns>
        public StyledProperty<TValue> RegisterAttached() => RegisterAttached<TClass>();

/*
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
*/
/*
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
*/
        DependencyPropertyConfigurator<TClass,TValue> Do(Action action)
        {
            action();
            return this;
        }

        /// <summary>
        ///     Gets or sets a value that indicates whether a dependency property potentially
        ///     affects the measure pass during layout engine operations.
        /// </summary>
        public DependencyPropertyConfigurator<TClass, TValue> AffectsMeasure => Do(() => throw new NotSupportedException());

        /// <summary>
        ///     Gets or sets a value that indicates whether a dependency property potentially
        ///     affects the arrange pass during layout engine operations.
        /// </summary>
        public DependencyPropertyConfigurator<TClass, TValue> AffectsArrange => Do(() => throw new NotSupportedException());

        /// <summary>
        ///     Gets or sets a value that indicates whether a dependency property potentially
        ///     affects the measure pass of its parent element's layout during layout engine
        ///     operations.
        /// </summary>
        public DependencyPropertyConfigurator<TClass, TValue> AffectsParentMeasure => Do(() => throw new NotSupportedException());

        /// <summary>
        ///     Dependency property potentially
        ///     affects the arrange pass of its parent element's layout during layout engine
        ///     operations.
        /// </summary>
        public DependencyPropertyConfigurator<TClass, TValue> AffectsParentArrange => Do(() => throw new NotSupportedException());

        /// <summary>
        ///     Gets or sets a value that indicates whether a dependency property potentially
        ///     affects the general layout in some way that does not specifically influence arrangement
        ///     or measurement, but would require a redraw.
        /// </summary>
        public DependencyPropertyConfigurator<TClass, TValue> AffectsRender => Do(() => throw new NotSupportedException());

        /// <summary>
        ///     The dependency property is inheritable.
        /// </summary>
        public DependencyPropertyConfigurator<TClass, TValue> Inherits => Do(() => _inherits = true);

        /// <summary>
        ///     Property value inheritance evaluation should span across certain content boundaries in the logical tree of elements.
        /// </summary>
        public DependencyPropertyConfigurator<TClass, TValue> OverridesInheritanceBehavior => Do(() => throw new NotSupportedException());

        /// <summary>
        ///     The dependency property does not support data binding.
        /// </summary>
        public DependencyPropertyConfigurator<TClass, TValue> NotDataBindable => Do(() => throw new NotSupportedException());

        /// <summary>
        ///     The property binds two-way by default.
        /// </summary>
        public DependencyPropertyConfigurator<TClass, TValue> BindsTwoWayByDefault => Do(() => _bindingMode = BindingMode.TwoWay);

        /// <summary>
        ///     This property contains journaling information that applications can or should store as part of a journaling implementation.
        /// </summary>
        public DependencyPropertyConfigurator<TClass, TValue> Journal => Do(() => throw new NotSupportedException());

        /// <summary>
        ///     Sub-properties of the dependency property do not affect the rendering of the containing object.
        /// </summary>
        public DependencyPropertyConfigurator<TClass, TValue> SubPropertiesDoNotAffectRender => Do(() => throw new NotSupportedException());

        /// <summary>
        ///     Gets or sets the default for System.Windows.Data.UpdateSourceTrigger to use when
        ///     bindings for the property with this metadata are applied, which have their System.Windows.Data.UpdateSourceTrigger
        ///     set to System.Windows.Data.UpdateSourceTrigger.Default.
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        //public DependencyPropertyConfigurator<TClass, TValue> DefaultUpdateSourceTrigger(UpdateSourceTrigger trigger)  => Do(() => _propertyMetadata.DefaultUpdateSourceTrigger  = trigger);

        /// <summary>
        ///     Represents the callback that is invoked when the effective property value of
        ///     a dependency property changes.
        /// </summary>
        /// <typeparam name="TSender"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public DependencyPropertyConfigurator<TClass,TValue> OnChange<TSender>(Action<TSender, bool> action)
            where TSender : IAvaloniaObject
        {
            _notifying += (s,b) => action((TSender)s,b);

            //_propertyMetadata.PropertyChangedCallback += (sender, args) =>
            //{
            //    if (sender is TSender typedSender)
            //    {
            //        action(typedSender, new DependencyPropertyChangedEventArgs<TValue>(args));
            //    }
            //    else
            //    {
            //        if(sender==null)
            //        {
            //            action(null, new DependencyPropertyChangedEventArgs<TValue>(args));
            //        }
            //    }

            //};

            return this;
        }

        /// <summary>
        ///     Represents the callback that is invoked when the effective property value of
        ///     a dependency property changes.
        /// </summary>
        /// <typeparam name="TSender"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        //public DependencyPropertyConfigurator<TClass,TValue> OnChange<TSender>(Action<TSender> action)
        //    where TSender : StyledElement
        //{
        //    _propertyMetadata.PropertyChangedCallback += (sender, args) =>
        //    {
        //        if (sender is TSender typedSender)
        //        {
        //            action(typedSender);
        //        }
        //    };

        //    return this;
        //}


        /// <summary>
        ///     Represents the callback that is invoked when the effective property value of
        ///     a dependency property changes.
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        //public DependencyPropertyConfigurator<TClass, TValue> OnChange(Action<IAvaloniaObject, bool> action)
        //    => OnChange<TClass>(action);

        /// <summary>
        ///     Represents the callback that is invoked when the effective property value of
        ///     a dependency property changes.
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public DependencyPropertyConfigurator<TClass, TValue> OnChange(Action<TClass,bool> action)
            => OnChange<TClass>(action);
        public DependencyPropertyConfigurator<TClass, TValue> OnChangeBeforeNotification(Action<TClass> action)
        {
            _notifying += (s,before) =>
            {
                if(before && s is TClass cls) action(cls);
            };
            return this;
        }
        public DependencyPropertyConfigurator<TClass, TValue> OnChangeAfterNotification(Action<TClass> action)
        {
            _notifying += (s,before) =>
            {
                if(!before && s is TClass cls) action(cls);
            };
            return this;
        }

        /// <summary>
        ///     Represents a method used as a callback that validates the effective value of
        ///     a dependency property.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public DependencyPropertyConfigurator<TClass,TValue> Validate(Func<TValue,bool> func)
        {
            _validate = func;

            return this;
        }

        /// <summary>
        ///     Sets the default value of the dependency property.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DependencyPropertyConfigurator<TClass,TValue> Default(TValue value)
        {
            _defaultValue = value;
            return this;
        }

    }
}