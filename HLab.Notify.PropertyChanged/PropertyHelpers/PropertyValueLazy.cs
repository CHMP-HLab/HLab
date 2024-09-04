using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HLab.Notify.PropertyChanged.PropertyHelpers.Helpers;

namespace HLab.Notify.PropertyChanged.PropertyHelpers;

public class PropertyValueLazy<T> : IPropertyValue<T>
{
    Func<object, T> _getter;
    readonly PropertyHolder<T> _holder;

    static readonly Func<PropertyHolder<T>, IPropertyValue<T>> _getPropertyValue;

    static PropertyValueLazy()
    {
        var propertyType = typeof(T);
        if (propertyType.IsPrimitive)
        {
            if (propertyType == typeof(int))
            {
                _getPropertyValue = e => (IPropertyValue<T>)new PropertyInt(e as PropertyHolder<int>);
                return;
            }
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
            }
        }

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
    }

    IPropertyValue<T> GetProperty() => _getPropertyValue(_holder);

    public T Get()
    {
        var property = GetProperty();
        var value = _getter == null ? default(T) : _getter(_holder.Parent);
        _holder.SetProperty(property);
        property.Set(value);
        return value;
    }

    public bool Set(T value)
    {
        var property = GetProperty();
        _holder.SetProperty(property);
        var ret = property.Set(value);
        return ret;
    }
    public bool Set(Func<object, T> value)
    {
        _getter = value;
        return true;
    }

    public PropertyValueLazy(PropertyHolder<T> holder, Func<object, T> getter)
    {
        _holder = holder;
        _getter = getter;
    }
}