using System;
using System.ComponentModel;
using HLab.Notify.PropertyChanged.PropertyHelpers;

namespace HLab.Notify.PropertyChanged
{
        public class PropertyDateTime : IPropertyValue<DateTime>
        {
            private readonly object _lock = new object();

            private DateTime _value;

            public DateTime Get()
            {
                lock (_lock)
                    return _value;
            }

            public bool Set(Func<object, DateTime> setter) => Set(setter(_holder.Parent));

        public bool Set(DateTime value)
            {
                lock (_lock)
                {
                    if (_value == value) return false;
                    _value = value;
                }

                _holder.OnPropertyChanged();
                return true;
            }

            private readonly PropertyHolder<DateTime> _holder;
            public PropertyDateTime(PropertyHolder<DateTime> holder)
            {
                _holder = holder;
            }
        }
    
}
