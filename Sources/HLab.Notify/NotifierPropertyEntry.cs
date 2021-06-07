/*
  HLab.Notify.4
  Copyright (c) 2017 Mathieu GRENET.  All right reserved.

  This file is part of HLab.Notify.4.

    HLab.Notify.4 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    HLab.Notify.4 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MouseControl.  If not, see <http://www.gnu.org/licenses/>.

	  mailto:mathieu@mgth.fr
	  http://www.mgth.fr
*/
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
using HLab.Base;
using HLab.Base.Extensions;
using HLab.DependencyInjection.Annotations;
using HLab.Notify.Annotations;

namespace HLab.Notify
{
    public class NotifierPropertyEntry<T> : INotifierPropertyEntry<T>
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandler RegisterValue;

        [Import]
        private readonly IEventHandlerService _eventHandlerService;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            _eventHandlerService.Invoke(PropertyChanged,_target,args);
        }

        protected virtual void OnRegisterValue(PropertyChangedEventArgs args)
        {
            RegisterValue?.Invoke(_target, args);
             //_eventHandlerService.Invoke(RegisterValue, _target,args);
       }

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        INotifierProperty INotifierPropertyEntry.Property => Property;
        public INotifierProperty<T> Property { get; }

        private readonly INotifyPropertyChanged _target;
        public NotifierPropertyEntry(INotifyPropertyChanged target, INotifierProperty<T> property)
        {
            _target = target;
            Property = property;
        }

        public T GetValue()
        {
            if(_value.HasGetter)
                return _value.Get();
            else if(Property is INotifierPropertyReflexion<T> npr)
            {
                return npr.PropertyInfo.GetValue(_target).Cast<T>();
            }
            return default(T);
        }

        object INotifierPropertyEntry.GetValue() => GetValue();



        public bool IsSet => _value.IsSet;

        public void Update(bool notify = false)
        {
            if (!_value.HasGetter && Property is INotifierPropertyReflexion<T> npr)
            {
                var pi = npr.PropertyInfo;
                var v = pi.GetValue(_target).Cast<T>();
                if (!_value.HasGetter)
                {
                    _value.SetGetter(o => pi.GetValue(_target).Cast<T>(),v);
                }

                OnValueChanged(default(T),v, notify);
                //OnValueChanged(default(T), default(T), notify);
            }
            else
            {
                _value.Reset((o,n)=> OnValueChanged(o, n, true));
            }
        }


        private void OnValueChanged(object old, object value, bool notify)
        {
            Property.AddOneToMany(old.Cast<T>(), value.Cast<T>(), _target);

            var arg = new NotifierPropertyChangedEventArgs(this, old, value);

            OnRegisterValue(arg);

            if (!notify) return;

            OnPropertyChanged(arg);
        }

        private readonly IValueHolder<T> _value = new LazyValueHolder<T>();

        //public Func<Func<T, T>, T> Get { get; private set; }

        private readonly Func<T> _locate;

        public T Locate(Func<T, T> getter = null, Action<T,T> postUpdateAction = null)
        {
            Func<T, T> g;
            if (getter == null)
                g = (o => _locate()); 
            else
                g = (o => getter(_locate()));

            return _value.Get(g,postUpdateAction);
        }
        public T Get(Func<T, T> getter, Action<T,T> postUpdateAction = null)
        {
            return _value.Get(getter,postUpdateAction);
        }
        public bool Set(object value, bool alwaysNotify, Action<object, object> postUpdateAction = null)
        {
            return Set((T)value,alwaysNotify,(o,n) => postUpdateAction?.Invoke(o.Cast<T>(),n.Cast<T>()));
        }

        public bool Set(T value, bool alwaysNotify, Action<T, T> postUpdateAction = null)
        {
            return _value.Set(o => value,(o,n) =>
            {
                postUpdateAction?.Invoke(o.Cast<T>(), n.Cast<T>());
                OnValueChanged(o,n,alwaysNotify);
            });
        }

        public void SetInitialValue(object value)
        {
            SetInitialValue(value.Cast<T>());
        }

        public void SetInitialValue(T value)
        {
            //_value.Set(o => value);
            Set(value, true);
        }

        public void OnTrigger(object sender, PropertyChangedEventArgs args)
        {
            //if (!IsSet)
            //{
            //    OnPropertyChanged(new NotifierPropertyChangedEventArgs(this, null, Value));
            //    return;
            //}



            if (_value.Get() is ITriggerable t)
            {
                t.OnTriggered();
                return;
            }


            Update(true);
        }

        public TriggerOnEntry Subscribe(PropertyChangedEventHandler handler, TriggerPath path)
        {
            var e = new TriggerOnEntryNotifier(handler, this, path);
            _triggerOnEntries.Add(e);
            return e;
        }

        private readonly ConcurrentBag<TriggerOnEntry> _triggerOnEntries = new ConcurrentBag<TriggerOnEntry>();
    }

}