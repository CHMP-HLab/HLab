using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using HLab.Notify.Annotations;

namespace HLab.Notify.PropertyChanged.NotifyParsers
{

    public partial class NotifyClassHelper : INotifyClassHelper
    {
        private static readonly ConditionalWeakTable<object, INotifyClassHelper> 
            Cache = new ConditionalWeakTable<object, INotifyClassHelper>();
        public static INotifyClassHelper GetExistingHelper(object target) => Cache.TryGetValue(target, out var p) ? p : null;

        public static INotifyClassHelper GetHelper(object target) => Cache.GetValue(target, (o) => new NotifyClassHelper(o).Init());

        public static INotifyClassHelper GetParserUninitialized(object target) => Cache.GetValue(target, (o) => new NotifyClassHelper(o));

        private readonly object _target;
        private NotifyClassHelper(object target)
        {
            _target = target;
        }

        public NotifyClassHelper Init()
        {
            Debug.Assert(_initialized == false);

            if(_target is INotifyPropertyChanged tpc)
                tpc.PropertyChanged += TargetPropertyChanged;

            if (_target is INotifyCollectionChanged tcc)
            {
                _dict.GetOrAdd("Item", n => new CollectionEntry(tcc));
            }
            _initialized = true;

            return this;
        }




        private readonly ConcurrentDictionary<string,IPropertyEntry> _dict = new ConcurrentDictionary<string, IPropertyEntry>();

        public IPropertyEntry GetPropertyEntry(string name) => _dict.GetOrAdd(name, n => new PropertyEntry(_target, n));

        public ITriggerEntry GetTrigger(TriggerPath path, ExtendedPropertyChangedEventHandler handler) => GetPropertyEntry(path.PropertyName).GetTrigger(path.Next, handler);

        public IEnumerable<IPropertyEntry> LinkedProperties()
        {
            foreach (var p in _dict.Values)
            {
                if(p.Linked) yield return p;
            }
        }
        public IEnumerable<IPropertyEntry> Properties()
        {
            return _dict.Values;
        }

        private bool _initialized = false;
        public void Initialize<T>() where T : class, INotifyPropertyChanged
        {
            if (!_initialized)
            {
                Init();
            }
            
            H<T>.InitializeAction.Value((T)_target, this);
        }



        private event PropertyChangedEventHandler Handler;
        public void AddHandler(PropertyChangedEventHandler value)
        {
            Handler += value;
        }

        public void RemoveHandler(PropertyChangedEventHandler value)
        {
            Handler -= value;
        }

        public void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            Handler?.Invoke(_target, args);
        }

        private void TargetPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_dict.TryGetValue(args.PropertyName, out var propertyEntry))
            {
                propertyEntry.TargetPropertyChanged(_target,args);
            }
        }
    }
}