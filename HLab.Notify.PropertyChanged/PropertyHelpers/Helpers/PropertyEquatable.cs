using System;
using System.Collections;
using System.ComponentModel;
using HLab.Notify.PropertyChanged.PropertyHelpers;

namespace HLab.Notify.PropertyChanged
{
    public class PropertyStructuralEquatable<T> : IPropertyValue<T>
        where T : IStructuralEquatable
    {
        private T _value;
        public T Get() => _value;
        public bool Set(Func<object, T> setter)
            => Set(setter(_holder.Parent));

        public bool Set(T value)
        {
            if (_value == null)
            {
                if(value == null) return false;
                _value = value;
                _holder.OnPropertyChanged();
                return true;
            }

            if (!_value.Equals(value, StructuralComparisons.StructuralEqualityComparer))
            {
                _value = value;
                _holder.OnPropertyChanged();
                return true;
            }
            else return false;
        }

        private readonly PropertyHolder<T> _holder;
        public PropertyStructuralEquatable(PropertyHolder<T> holder)
        {
            _holder = holder;
        }
    }
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
