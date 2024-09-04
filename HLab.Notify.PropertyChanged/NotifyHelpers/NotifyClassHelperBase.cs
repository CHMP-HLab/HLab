using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using HLab.Base;

namespace HLab.Notify.PropertyChanged.NotifyHelpers;

public class NotifyClassHelperBase : INotifyClassHelper
{
    static readonly ConditionalWeakTable<object, INotifyClassHelper> Cache = new();
    public static INotifyClassHelper GetExistingHelper(object target) => Cache.TryGetValue(target, out var p) ? p : null;
        
    public static INotifyClassHelper GetHelper(object target)
    {
        if (target is INotifyPropertyChangedWithHelper targetWithHelper) return targetWithHelper.ClassHelper;
        return Cache.GetValue(target, (o) => new NotifyClassHelperGeneric(o));
    }

    public static INotifyClassHelper GetHelper(INotifyPropertyChangedWithHelper target) => target.ClassHelper;


    event PropertyChangedEventHandler Handler;
    readonly Suspender _suspender = new();

    protected object Target { get; }
    readonly ConcurrentDictionary<string, IPropertyEntry> _propertiesDictionary = new();

    public SuspenderToken GetSuspender() => _suspender.Get();



    protected NotifyClassHelperBase(object target)
    {
        Target = target;

        if (target is INotifyCollectionChanged tcc)
        {
            _propertiesDictionary.GetOrAdd("Item", n => new CollectionPropertyEntry(tcc));
        }
    }


    public IEnumerable<IPropertyEntry> LinkedProperties() => _propertiesDictionary.Values.Where(p => p.IsLinked());

    public IEnumerable<IPropertyEntry> Properties() => _propertiesDictionary.Values;


    public IPropertyEntry GetPropertyEntry(string name) => _propertiesDictionary.GetOrAdd(name, n => new NotifyClassHelper.PropertyEntry(Target, n));

    protected bool TryGetPropertyEntry(string name, out IPropertyEntry propertyEntry) => _propertiesDictionary.TryGetValue(name, out propertyEntry);

    public void Initialize<T>() where T : class, INotifyPropertyChangedWithHelper
    {
        H<T>.InitializeAction.Value((T)Target);
    }
    public void AddHandler(PropertyChangedEventHandler value) => Handler += value;

    public void RemoveHandler(PropertyChangedEventHandler value) => Handler -= value;

    public virtual void OnPropertyChanged(PropertyChangedEventArgs args)
    {
        using var s = _suspender.Get();
        s.EnqueueAction(() =>
        {
            try
            {
                Handler?.Invoke(Target, args);
            }
            catch (Exception e)
            {

            }
        });
    }

}