using System;
using System.Threading;

namespace HLab.Notify.PropertyChanged.PropertyHelpers.Helpers
{
        public class PropertyBool : IPropertyValue<bool>
        {
            private volatile int _value;
            public bool Get() => _value != 0;

            public bool Set(bool value)
            {
                var v = value ? 1 : 0;

                if (_value == v) return false;

                var old = Interlocked.Exchange(ref _value, v);
                if (old == v) return false;
                _holder.OnPropertyChanged();
                return true;
            }

            public bool Set(Func<object, bool> setter) => Set(setter(_holder.Parent));

            private readonly PropertyHolder<bool> _holder;

            public PropertyBool(PropertyHolder<bool> holder)
            {
                _holder = holder;
            }
        }
    
}
