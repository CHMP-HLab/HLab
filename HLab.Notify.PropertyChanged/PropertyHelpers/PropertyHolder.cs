using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using HLab.Notify.PropertyChanged.NotifyHelpers;

namespace HLab.Notify.PropertyChanged.PropertyHelpers;

public interface IPropertyHolderN
{
    object Value{ get; set; }
    //bool Enabled { get; set; }
    //bool Mandatory { get; set; }
}

internal enum PropertyState
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

    readonly IProperty<T> _value = H.Property<T>();

    public bool Enabled
    {
        get => _enabled.Get();
        set => _enabled.Set(value);
    }
    object IPropertyHolderN.Value { get => Value; set => Value = (T)value; }

    readonly IProperty<bool> _enabled = H.Property<bool>();
    public event PropertyChangedEventHandler PropertyChanged;


    public INotifyClassHelper ClassHelper { get; }
}

public abstract class PropertyHolder : ChildObject
{
    protected PropertyHolder(PropertyActivator activator) : base(activator)
    {
    }
}

public class PropertyHolderInt : PropertyValueHolder<int>
{
    public PropertyHolderInt(PropertyActivator activator) : base(activator)
    {
    }

    int _value;

    public override int GetNoCheck() => _value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Set(int value)
    {
        if (_value == value) return false;

        var old = Interlocked.Exchange(ref _value, value);
        if (old == value) return false;

        OnPropertyChanged(old,value);
        return true;
    }
}
public abstract class PropertyValueHolder<T> : ChildObject, IProperty<T>
{
    public static IProperty<T> Create(PropertyActivator activator) => GetPropertyHolder(activator);

    static readonly Func<PropertyActivator,IProperty<T>> GetPropertyHolder;

    static PropertyValueHolder()
    {
        var propertyType = typeof(T);
        if (propertyType.IsPrimitive)
        {
            if (propertyType == typeof(int))
            {
                GetPropertyHolder = a => (IProperty<T>)new PropertyHolderInt(a);
                return;
            }
            /*
            if (propertyType == typeof(long))
            {
                _getPropertyValue = e => (IPropertyValue<T>)new PropertyLong(e as PropertyHolder<long>);
                return;
            }

            if (propertyType == typeof(float))
            {
                _getPropertyValue = e => (IPropertyValue<T>)new PropertyFloat(e as PropertyHolder<float>);
                return;
            }

            if (propertyType == typeof(double))
            {
                _getPropertyValue = e => (IPropertyValue<T>)new PropertyDouble(e as PropertyHolder<double>);
                return;
            }

            if (propertyType == typeof(bool))
            {
                _getPropertyValue = e => (IPropertyValue<T>)new PropertyBool(e as PropertyHolder<bool>);
                return;
            }*/
        }
        /*
        if (propertyType == typeof(string))
        {
            _getPropertyValue = e => (IPropertyValue<T>)new PropertyString(e as PropertyHolder<string>);
            return;
        }

        Type makeType = null;
        if (propertyType.IsInterface)
            makeType = typeof(PropertyStruct<T>);//TODO maybe we can optimize
        else if (propertyType.IsArray)
            makeType = typeof(PropertyStructuralEquatable<>).MakeGenericType(propertyType);
        else if (propertyType.IsClass)
            makeType = typeof(PropertyObject<>).MakeGenericType(propertyType);
        else if (propertyType.IsEnum && Enum.GetUnderlyingType(propertyType) == typeof(int))
            makeType = typeof(PropertyEnum<>).MakeGenericType(propertyType);
        else if (propertyType.IsValueType)
            makeType = typeof(PropertyStruct<T>);//.MakeGenericType(propertyType);
        else
        {
            throw new ArgumentException("Invalid type");
        }
        var argumentTypes = new[] { typeof(PropertyHolder<T>) };

        var constructor = makeType.GetConstructor(
            BindingFlags.Instance | BindingFlags.Public,
            null,
            CallingConventions.HasThis,
            argumentTypes,
            new ParameterModifier[0]);

        var lamdaParameterExpressions = new[]
        {
                Expression.Parameter(typeof(PropertyHolder<T>), "holder"),
            };

        var constructorParameterExpressions = lamdaParameterExpressions
            .Take(argumentTypes.Length)
            .ToArray();

        var constructorCallExpression =
            Expression.New(constructor, constructorParameterExpressions);
        var func = Expression
            .Lambda<Func<PropertyHolder<T>, IPropertyValue<T>>>(constructorCallExpression, lamdaParameterExpressions)
            .Compile();

        _getPropertyValue = func;
        */

        GetPropertyHolder = a => (IProperty<T>)new PropertyHolder<T>(a);
    }



    protected PropertyValueHolder(PropertyActivator activator) : base(activator)
    {
    }

    public T Value
    {
        get => Get();
        set => Set(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public  T Get() => GetNoCheck();

    public abstract T GetNoCheck();

#if DEBUG
    [DebuggerStepThrough]
    public T Get([CallerMemberName] string name = null)
    {
        if (name != null && name != "SetOneToMany" && name != "Set")
            Debug.Assert(name == this.Name);

        return GetNoCheck();
    }
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract bool Set(T value);

}

public class PropertyHolder<T> : ChildObject, IProperty<T>
{
    protected internal IPropertyValue<T> PropertyValue;

    public PropertyHolder(PropertyActivator activator) : base(activator)
    {
    }

    public T Value
    {
        get => PropertyValue.Get();
        set => PropertyValue.Set(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Get() => PropertyValue == null ? default : PropertyValue.Get();

    public T GetNoCheck() => PropertyValue == null ? default : PropertyValue.Get();

#if DEBUG
    [DebuggerStepThrough]
    public T Get([CallerMemberName] string name = null)
    {
        if (name != null && name != "SetOneToMany" && name != "Set")
            Debug.Assert(name == this.Name);

        return PropertyValue == null ? default : PropertyValue.Get();
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