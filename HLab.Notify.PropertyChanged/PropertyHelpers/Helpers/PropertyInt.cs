using System;
using System.Threading;

namespace HLab.Notify.PropertyChanged.PropertyHelpers.Helpers;

public class PropertyInt : IPropertyValue<int>
{
    volatile int _value;
    public int Get() => _value;

    public bool Set(int value)
    {
        if (_value == value) return false;

        var old = Interlocked.Exchange(ref _value, value);
        if (old == value) return false;

        _holder.OnPropertyChanged(old,value);
        return true;
    }

    public bool Set(Func<object, int> setter) => Set(setter(_holder.Parent));

    readonly PropertyHolder<int> _holder;
    public PropertyInt(PropertyHolder<int> holder)
    {
        _holder = holder;
    }
}