using System.Threading;

namespace HLab.Notify.PropertyChanged.PropertyHelpers.Helpers;

public class PropertyDouble : IPropertyValue<double>
{
    double _value;
    public double Get() => _value;

    public bool Set(double value)
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

    public bool Set(System.Func<object, double> setter)
    {
        return Set(setter(_holder.Parent));
    }

    readonly PropertyHolder<double> _holder;

    public PropertyDouble(PropertyHolder<double> holder)
    {
        _holder = holder;
    }
}