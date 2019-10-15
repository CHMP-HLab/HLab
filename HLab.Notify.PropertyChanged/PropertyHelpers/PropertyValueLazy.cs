using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HLab.Notify.PropertyChanged
{
    public class PropertyValueLazy<T> : IPropertyValue<T>
    {
        private Func<object, T> _getter;
        private readonly PropertyHolder<T> _holder;

        private static readonly Func<PropertyHolder<T>, IPropertyValue<T>> GetPropertyValue;

        static PropertyValueLazy()
        {
            var t = typeof(T);

            if (t == typeof(int))
                GetPropertyValue = e => (IPropertyValue<T>)new PropertyInt(e as PropertyHolder<int>);
            else if (t == typeof(long))
                GetPropertyValue = e => (IPropertyValue<T>)new PropertyLong(e as PropertyHolder<long>);
            else if (t == typeof(float))
                GetPropertyValue = e => (IPropertyValue<T>)new PropertyFloat(e as PropertyHolder<float>);
            else if (t == typeof(double))
                GetPropertyValue = e => (IPropertyValue<T>)new PropertyDouble(e as PropertyHolder<double>);
            else if (t == typeof(bool))
                GetPropertyValue = e => (IPropertyValue<T>)new PropertyBool(e as PropertyHolder<bool>);
            else if (t == typeof(string))
                GetPropertyValue = e => (IPropertyValue<T>)new PropertyString(e as PropertyHolder<string>);
            else
            {
                var propertyType = typeof(T);
                Type makeType = null;
                if (propertyType.IsClass)
                    makeType = typeof(PropertyObject<>).MakeGenericType(propertyType);
                else if (propertyType.IsEnum && Enum.GetUnderlyingType(propertyType) == typeof(int))
                    makeType = typeof(PropertyEnum<>).MakeGenericType(propertyType);
                else
                    makeType = typeof(PropertyStruct<>).MakeGenericType(propertyType);

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

                GetPropertyValue = func;

            }

        }

        private IPropertyValue<T> GetProperty() => GetPropertyValue(_holder);

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
}