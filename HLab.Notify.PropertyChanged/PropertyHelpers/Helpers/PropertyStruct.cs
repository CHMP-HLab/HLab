using System;

namespace HLab.Notify.PropertyChanged.PropertyHelpers.Helpers;

public class PropertyStruct<T> : IPropertyValue<T>
{
    readonly object _lock = new object();

    T _value;

    public T Get()
    {
        lock (_lock)
            return _value;
    }

    public bool Set(T value)
    {
        T old;
        lock (_lock)
        {
            if (Equals(_value, value)) return false;
            old = _value;
            _value = value;
        }

        _holder.OnPropertyChanged(old,value);
        return true;
    }

    public bool Set(Func<object, T> setter)
    {
        return Set(setter(_holder.Parent));
    }

    readonly PropertyHolder<T> _holder;
    public PropertyStruct(PropertyHolder<T> holder)
    {
        _holder = holder;
    }
}