using System;
using System.ComponentModel;
using HLab.Notify.PropertyChanged.PropertyHelpers;

namespace HLab.Notify.PropertyChanged
{
    public class PropertyString : IPropertyValue<string>
    {
        private string _value;
        public string Get() => _value;

        public bool Set(string value)
        {
            if (_value != value)
            {
                var old = _value;
                _value = value;
                _holder.OnPropertyChanged(old,value);
                return true;
            }
            else return false;
        }

        public bool Set(Func<object, string> setter)
        {
            return Set(setter(_holder.Parent));
        }

        private readonly PropertyHolder<string> _holder;
        public PropertyString(PropertyHolder<string> holder)
        {
            _holder = holder;
        }
    }
}
