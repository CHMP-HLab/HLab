using System;
using System.ComponentModel;

namespace HLab.Notify.PropertyChanged
{
    public class PropertyEquatable<T> : IPropertyValue<IEquatable<T>>
    {
        private IEquatable<T> _value;
        public IEquatable<T> Get() => _value;
        public bool Set(Func<object, IEquatable<T>> setter)
            => Set(setter(_holder.Parent));

        public bool Set(IEquatable<T> value)
        {
            if (_value != value)
            {
                _value = value;
                _holder.OnPropertyChanged();
                return true;
            }
            else return false;
        }

        private readonly PropertyHolder<T> _holder;
        public PropertyEquatable(PropertyHolder<T> holder)
        {
            _holder = holder;
        }
    }
}
