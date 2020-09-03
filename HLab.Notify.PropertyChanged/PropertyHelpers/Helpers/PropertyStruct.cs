using System;

namespace HLab.Notify.PropertyChanged.PropertyHelpers
{
    public class PropertyStruct<T> : IPropertyValue<T>
    {
        private readonly object _lock = new object();

        private T _value;

        public T Get()
        {
            lock (_lock)
                return _value;
        }

        public bool Set(T value)
        {
            lock (_lock)
            {
                if (Equals(_value, value)) return false;
                _value = value;
            }

            _holder.OnPropertyChanged();
            return true;
        }

        public bool Set(Func<object, T> setter)
        {
            return Set(setter(_holder.Parent));
        }

        private readonly PropertyHolder<T> _holder;
        public PropertyStruct(PropertyHolder<T> holder)
        {
            _holder = holder;
        }
    }
}
