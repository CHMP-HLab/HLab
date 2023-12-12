using System;
using System.Threading;

namespace HLab.Notify.PropertyChanged.PropertyHelpers.Helpers;

public class PropertyLong : IPropertyValue<long>
{
    long _value;
    public long Get() => _value;

    public bool Set(long value)
    {
        if (_value == value) return false;

        var old = Interlocked.Exchange(ref _value, value);
        if (old != value)
        {
            _holder.OnPropertyChanged(old,value);
            return true;
        }
        else return false;
    }

    public bool Set(Func<object, long> setter)
    {
        return Set(setter(_holder.Parent));
    }

    readonly PropertyHolder<long> _holder;
    public PropertyLong(PropertyHolder<long> holder)
    {
        _holder = holder;
    }
}