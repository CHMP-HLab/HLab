﻿using System;
using System.Threading;

namespace HLab.Notify.PropertyChanged.PropertyHelpers.Helpers
{
    public class PropertyFloat : IPropertyValue<float>
    {
        private float _value;
        public float Get() => _value;

        public bool Set(float value)
        {
            if (_value == value) return false;

            var old = Interlocked.Exchange(ref _value, value);
            if (old != value)
            {
                _holder.OnPropertyChanged(old,value);
                return true;
            }
            else return false;
        }

        public bool Set(Func<object, float> setter)
        {
            return Set(setter(_holder.Parent));
        }

        private readonly PropertyHolder<float> _holder;

        public PropertyFloat(PropertyHolder<float> holder)
        {
            _holder = holder;
        }
    }
}