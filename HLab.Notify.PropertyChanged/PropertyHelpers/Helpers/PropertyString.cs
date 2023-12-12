using System;

namespace HLab.Notify.PropertyChanged.PropertyHelpers.Helpers;

public class PropertyString : IPropertyValue<string>
{
    string _value;
    public string Get() => _value;

    public bool Set(string value)
    {
        if (_value == value) return false;

        var old = _value;
        _value = value;
        _holder.OnPropertyChanged(old,value);
        return true;
    }

    public bool Set(Func<object, string> setter)
    {
        return Set(setter(_holder.Parent));
    }

    readonly PropertyHolder<string> _holder;
    public PropertyString(PropertyHolder<string> holder)
    {
        _holder = holder;
    }
}