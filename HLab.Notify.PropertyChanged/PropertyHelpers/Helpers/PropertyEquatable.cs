using System;
using System.Collections;

namespace HLab.Notify.PropertyChanged.PropertyHelpers.Helpers;

public class PropertyStructuralEquatable<T> : IPropertyValue<T>
    where T : IStructuralEquatable
{
    T _value;
    public T Get() => _value;
    public bool Set(Func<object, T> setter)
        => Set(setter(_holder.Parent));

    public bool Set(T value)
    {
        if (_value == null)
        {
            if(value == null) return false;
            _value = value;
            _holder.OnPropertyChanged(default(T),value);
            return true;
        }

        if (!_value.Equals(value, StructuralComparisons.StructuralEqualityComparer))
        {
            var old = _value;
            _value = value;
            _holder.OnPropertyChanged(old,value);
            return true;
        }
        else return false;
    }

    readonly PropertyHolder<T> _holder;
    public PropertyStructuralEquatable(PropertyHolder<T> holder)
    {
        _holder = holder;
    }
}
public class PropertyEquatable<T> : IPropertyValue<IEquatable<T>>
{
    IEquatable<T> _value;
    public IEquatable<T> Get() => _value;
    public bool Set(Func<object, IEquatable<T>> setter)
        => Set(setter(_holder.Parent));

    public bool Set(IEquatable<T> value)
    {
        if (_value != value)
        {
            var old = _value;
            _value = value;
            _holder.OnPropertyChanged(old,value);
            return true;
        }
        else return false;
    }

    readonly PropertyHolder<T> _holder;
    public PropertyEquatable(PropertyHolder<T> holder)
    {
        _holder = holder;
    }
}