using System;

namespace HLab.Notify.PropertyChanged.PropertyHelpers.Helpers
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
                DateTime oldValue;
                lock (_lock)
                {
                    if (_value == value) return false;
                    oldValue = _value;
                    _value = value;
                }

                _holder.OnPropertyChanged(oldValue,value);
                return true;
            }

            private readonly PropertyHolder<DateTime> _holder;
            public PropertyDateTime(PropertyHolder<DateTime> holder)
            {
                _holder = holder;
            }
        }
    
}
