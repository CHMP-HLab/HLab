using System;
using System.ComponentModel;
using System.Threading;
using HLab.Notify.PropertyChanged.PropertyHelpers;

namespace HLab.Notify.PropertyChanged
{
    public class PropertyLong : IPropertyValue<long>
    {
        private long _value;
        public long Get() => _value;

        public bool Set(long value)
        {
            if (_value == value) return false;

            var old = Interlocked.Exchange(ref _value, value);
            if (old != value)
            {
                _holder.OnPropertyChanged();
                return true;
            }
            else return false;
        }

        public bool Set(Func<object, long> setter)
        {
            return Set(setter(_holder.Parent));
        }

        private readonly PropertyHolder<long> _holder;
        public PropertyLong(PropertyHolder<long> holder)
        {
            _holder = holder;
        }
    }
}