using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using HLab.Notify.Annotations;

namespace HLab.Notify.PropertyChanged.NotifyParsers
{

    public class NotifyClassHelperBase : INotifyClassHelper
    {
        private static readonly ConditionalWeakTable<object, INotifyClassHelper> 
            Cache = new ConditionalWeakTable<object, INotifyClassHelper>();
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
                Dict.GetOrAdd("Item", n => new CollectionEntry(tcc));
            }
        }

        protected readonly ConcurrentDictionary<string,IPropertyEntry> Dict = new ConcurrentDictionary<string, IPropertyEntry>();

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

        public ITriggerEntry GetTrigger(TriggerPath path, EventHandler<ExtendedPropertyChangedEventArgs> handler) => GetPropertyEntry(path.PropertyName).GetTrigger(path.Next, handler);

        public void Initialize<T>() where T : class, INotifyPropertyChangedWithHelper
        {
                H<T>.InitializeAction.Value.Activator((T)Target);
        }
        public void AddHandler(PropertyChangedEventHandler value) => Handler += value;

        public void RemoveHandler(PropertyChangedEventHandler value) => Handler -= value;

        public virtual void OnPropertyChanged(PropertyChangedEventArgs args) => Handler?.Invoke(Target, args);
        public void Dispose()
        {
            foreach (var name in Dict.Keys.ToList())
            {
                if(Dict.Remove(name, out var entry))
                    entry.Dispose();
            }
        }
    }

    public partial class NotifyClassHelperGeneric : NotifyClassHelperBase
    {
        public NotifyClassHelperGeneric(object target) : base(target)
        {
            if(target is INotifyPropertyChanged tpc)
                tpc.PropertyChanged += TargetPropertyChanged;
        }

        private void TargetPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (Dict.TryGetValue(args.PropertyName, out var propertyEntry))
            {
                propertyEntry.TargetPropertyChanged(Target,args);
            }
        }
    }

    public partial class NotifyClassHelper : NotifyClassHelperBase
    {
        public NotifyClassHelper(object target) : base(target) { }

        public override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);
            if (Dict.TryGetValue(args.PropertyName, out var propertyEntry))
            {
                propertyEntry.TargetPropertyChanged(Target,args);
            }
        }
    }
}