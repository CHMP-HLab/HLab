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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using HLab.DependencyInjection.Annotations;
using HLab.DependencyInjection;
using HLab.Notify.Annotations;
using HLab.Base;

namespace HLab.Notify
{


    [Export(typeof(INotifier)), GenericAsTarget]
    public partial class Notifier<TClass> : INotifier<TClass>, IInitializer
        where TClass : class, INotifyPropertyChanged
    {
        [Import]
        private readonly  INotifierClass<TClass> _class;

        [Import]
        private readonly IEventHandlerService _eventHandlerService;

        public INotifier Subscribe()
        {
           if (Target is NotifierObject obj)
               obj.OnSubscribe(this);
            _class.Subscribe(this);
            return this;
        }

        public INotifierPropertyEntry<T> GetPropertyEntry<T>(PropertyInfo propertyInfo)
        {
            var property = _class.GetProperty<T>(propertyInfo);
            var entry = GetPropertyEntry(property);
            return entry;
        }

        public INotifierPropertyEntry<T> GetPropertyEntry<T>(string propertyName) 
            => GetPropertyEntry(_class.GetProperty<T>(propertyName));
        public INotifierPropertyEntry GetPropertyEntry(string propertyName)
            => GetPropertyEntry(_class.GetProperty(propertyName));

        public INotifierPropertyEntry GetPropertyEntry(INotifierProperty property)
            => _propertyEntries.GetOrAdd(property, p =>
            {
                var e = property.GetNewEntry(Target);
                e.PropertyChanged += (o, a) => OnPropertyChanged(a);
                return e;
            });

        public INotifierPropertyEntry<T> GetPropertyEntry<T>(INotifierProperty<T> property)
        {
            return (INotifierPropertyEntry<T>) _propertyEntries.GetOrAdd(property, p =>
            {
                var target = Target;
                var e = property.GetNewEntry(target);
                e.PropertyChanged += (o, a) => OnPropertyChanged(a);
                return e;
            });
        }


        private readonly ConcurrentDictionary<INotifierProperty, INotifierPropertyEntry> _propertyEntries =
            new ConcurrentDictionary<INotifierProperty, INotifierPropertyEntry>();

        public T Get<T>(Func<T> getter, string propertyName)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(propertyName), "propertyName cannot be null or empty");
            return GetPropertyEntry<T>(propertyName).Get(o => getter());
        }

        public T Get<T>(Func<T, T> getter, string propertyName)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(propertyName), "propertyName cannot be null or empty");
            return GetPropertyEntry<T>(propertyName).Get(getter);
        }
        private T Get<T>(Func<T> getter, INotifierProperty<T> property/*, Action postUpdateAction = null*/)
        {
                return GetPropertyEntry<T>(property).Get(o => getter());
        }
        public T Locate<T>(Func<T, T> getter, string propertyName)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(propertyName), "propertyName cannot be null or empty");
            return GetPropertyEntry<T>(propertyName).Locate(getter);
        }
        public T Locate<T>(string propertyName)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(propertyName), "propertyName cannot be null or empty");
            return GetPropertyEntry<T>(propertyName).Locate();
        }


        public bool Set<T>(T value, string propertyName, Action<T, T> postUpdateAction = null)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(propertyName), "propertyName cannot be null or empty");

            return Set(value, _class.GetProperty<T>(propertyName), postUpdateAction);
        }

        private bool Set<T>(T value, INotifierProperty<T> property, Action<T, T> postUpdateAction = null)
        {
            //using (Suspend.Get())
            {
                var entry = GetPropertyEntry(property);
                    
                //(INotifierPropertyEntry<T>)_propertyEntries.GetOrAdd(property, (oldValue) =>
                //{
                   
                //    return property.GetNewEntry(Target);
                //});
                return entry.Set(value, true, postUpdateAction);
            }
        }
        public bool SetOneToMany<TClass1, T>(T value, Func<T, IList<TClass1>> getCollection, [CallerMemberName] string propertyName = null)
            => SetOneToMany<T>(value, (Func<T, IList<TClass>>)getCollection, propertyName);

        public bool SetOneToMany<T>(T value, Func<T, IList<TClass>> getCollection, [CallerMemberName] string propertyName = null)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(propertyName), "propertyName cannot be null or empty");

            return Set(value, _class.GetProperty<T>(propertyName), (oldValue, newValue) =>
            {
                if(oldValue!=null) getCollection(oldValue).Remove(Target);
                if(newValue!=null) getCollection(newValue).Add(Target);
            });
        }

        internal bool IsSet<T>(PropertyInfo property) => IsSet(_class.GetProperty<T>(property));

        private bool IsSet(INotifierProperty property)
        {
            if (_propertyEntries.TryGetValue(property, out var entry))
            {
                return entry.IsSet;
            }
            return false;
        }


        private void Update(INotifierProperty property)
        {
            if(_propertyEntries.TryGetValue(property, out var entry))
                entry.Update(true);
        }


        public bool Subscribed { get; } = false;

        public void Subscribe(PropertyChangedEventHandler handler, TriggerPath path)
        {
            Debug.Assert(path != null);
            GetPropertyEntry(_class.GetProperty(path.PropertyName)).Subscribe( handler, path.Next );
        }


        public void Subscribe(INotifierProperty triggeredProperty, INotifierProperty targetProperty, TriggerPath path)
        {
            GetPropertyEntry(targetProperty).Subscribe(GetPropertyEntry(triggeredProperty).OnTrigger, path);
        }

        public void Initialize(IRuntimeImportContext ctx, object[] args)
        {
            var target = ctx.GetTarget<TClass>();
            _target = new WeakReference<TClass>(target);
        }

    }
}
