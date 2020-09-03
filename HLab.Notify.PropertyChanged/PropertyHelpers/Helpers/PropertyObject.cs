using System;
using System.ComponentModel;
using System.Threading;
using HLab.Notify.PropertyChanged.PropertyHelpers;

namespace HLab.Notify.PropertyChanged
{
    public class PropertyObject<T> : IPropertyValue<T>
        where T : class
    {
        private T _value;
        public T Get() => _value;

        public bool Set(T value)
        {
            if (ReferenceEquals(_value, value)) return false;

            var old = Interlocked.Exchange(ref _value, value);
            if (!ReferenceEquals(old, value))
            {
                _holder.OnPropertyChanged();
                return true;
            }
            else return false;
        }

        public bool Set(System.Func<object, T> setter) => Set(setter(_holder.Parent));

        private readonly PropertyHolder<T> _holder;
        public PropertyObject(PropertyHolder<T> holder)
        {
            _holder = holder;
        }
    }
}
