using System;

namespace HLab.Notify.PropertyChanged.PropertyHelpers.Helpers;

public class PropertyDateTime : IPropertyValue<DateTime>
{
    readonly object _lock = new object();

    DateTime _value;

    public DateTime Get()
    {
        lock (_lock)
            return _value;
    }

    public bool Set(Func<object, DateTime> setter) => Set(setter(_holder.Parent));

    public bool Set(DateTime value)
    {
        DateTime oldValue;
        lock (_lock)
        {
            if (_value == value) return false;
            oldValue = _value;
            _value = value;
        }

        _holder.OnPropertyChanged(oldValue,value);
        return true;
    }

    readonly PropertyHolder<DateTime> _holder;
    public PropertyDateTime(PropertyHolder<DateTime> holder)
    {
        _holder = holder;
    }
}