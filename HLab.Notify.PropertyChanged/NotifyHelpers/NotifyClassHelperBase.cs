using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HLab.Base;
using HLab.Notify.Annotations;

namespace HLab.Notify.PropertyChanged.NotifyHelpers
{
    public class NotifyClassHelperBase : INotifyClassHelper
    {
        private readonly Suspender _suspender  = new();
        public SuspenderToken GetSuspender() => _suspender.Get();

        private static readonly ConditionalWeakTable<object, INotifyClassHelper> Cache = new();
        public static INotifyClassHelper GetExistingHelper(object target) => Cache.TryGetValue(target, out var p) ? p : null;
        public static INotifyClassHelper GetNewHelper(object target)
        {
            var h = new NotifyClassHelper(target);
            Cache.Add(target,h);
            return h;
        }

        public static INotifyClassHelper GetHelper(object target)
        {
            if (target is INotifyPropertyChangedWithHelper n) return n.ClassHelper; 
            return Cache.GetValue(target, (o) => new NotifyClassHelperGeneric(o));
        }
        public static INotifyClassHelper GetHelper(INotifyPropertyChangedWithHelper target) => target.ClassHelper; 



        protected readonly object Target;
        private event PropertyChangedEventHandler Handler;

        public NotifyClassHelperBase(object target)
        {
            Target = target;

            if (target is INotifyCollectionChanged tcc)
            {
                Dict.GetOrAdd("Item", n => new CollectionPropertyEntry(tcc));
            }
        }

        protected readonly ConcurrentDictionary<string,IPropertyEntry> Dict = new();

        public IEnumerable<IPropertyEntry> LinkedProperties()
        {
            foreach (var p in Dict.Values)
            {
                if(p.Linked) yield return p;
            }
        }
        public IEnumerable<IPropertyEntry> Properties()
        {
            return Dict.Values;
        }
        public IPropertyEntry GetPropertyEntry(string name) => Dict.GetOrAdd(name, n => new NotifyClassHelper.PropertyEntry(Target, n));

        public ITriggerEntry GetTrigger(TriggerPath path, EventHandler<ExtendedPropertyChangedEventArgs> handler)
        {
            return GetPropertyEntry(path.PropertyName).BuildTrigger(path.Next, handler);
        }
        public ITriggerEntry GetTriggerWithPath(TriggerPath path, EventHandler<ExtendedPropertyChangedEventArgs> handler)
        {
            return GetPropertyEntry(path.PropertyName).BuildTrigger(path.Next, handler);
        }

        public void Initialize<T>() where T : class, INotifyPropertyChangedWithHelper
        {
                H<T>.InitializeAction.Value((T)Target);
        }
        public void AddHandler(PropertyChangedEventHandler value) => Handler += value;

        public void RemoveHandler(PropertyChangedEventHandler value) => Handler -= value;

        public virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            using var s = GetSuspender();
            s.EnqueueAction(()=>Handler?.Invoke(Target, args));
        }

    }
}