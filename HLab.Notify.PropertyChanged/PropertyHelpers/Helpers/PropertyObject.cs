using System.Threading;

namespace HLab.Notify.PropertyChanged.PropertyHelpers.Helpers;

public class PropertyObject<T> : IPropertyValue<T>
    where T : class
{
    T _value;
    public T Get() => _value;

    public bool Set(T value)
    {
        if (ReferenceEquals(_value, value)) return false;

        var old = Interlocked.Exchange(ref _value, value);
        if (!ReferenceEquals(old, value))
        {
            _holder.OnPropertyChanged(old,value);
            return true;
        }
        else return false;
    }

    public bool Set(System.Func<object, T> setter) => Set(setter(_holder.Parent));

    readonly PropertyHolder<T> _holder;
    public PropertyObject(PropertyHolder<T> holder)
    {
        _holder = holder;
    }
}