using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace HLab.Notify.PropertyChanged.NotifyHelpers;

internal class CollectionPropertyEntry : IPropertyEntry
{
    readonly INotifyCollectionChanged _target;
    public string Name => "Item";

    public CollectionPropertyEntry(INotifyCollectionChanged target)
    {
        _target = target;
        target.CollectionChanged += TargetOnCollectionChanged;
    }

    void AddItem(object sender, object item)
    {
        ExtendedPropertyChanged?.Invoke(sender, new ExtendedPropertyChangedEventArgs("Item", null, item));
    }

    void RemoveItem(object sender, object item)
    {
        ExtendedPropertyChanged?.Invoke(sender, new ExtendedPropertyChangedEventArgs("Item", item, null));
    }

    void TargetOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (args.NewItems != null)
                    foreach (var item in args.NewItems)
                    {
                        AddItem(sender, item);
                    }
                else throw new Exception("NewItems was null");
                break;

            case NotifyCollectionChangedAction.Remove:
                if (args.OldItems != null)
                    foreach (var item in args.OldItems)
                    {
                        RemoveItem(sender, item);
                    }
                else throw new Exception("OldItems was null");
                break;

            case NotifyCollectionChangedAction.Move:
            case NotifyCollectionChangedAction.Replace:
            case NotifyCollectionChangedAction.Reset:
                ExtendedPropertyChanged?.Invoke(sender, new ExtendedPropertyChangedEventArgs("Item", null, null));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void InitialRegisterValue(Type type)
    {

    }

    public event EventHandler<ExtendedPropertyChangedEventArgs> ExtendedPropertyChanged;
    //public event EventHandler<ExtendedPropertyChangedEventArgs> RegisterValue;
    public void Link(EventHandler<ExtendedPropertyChangedEventArgs> handler)
    {
        if (_target is IEnumerable e)
            foreach (var obj in e) handler(this, new ExtendedPropertyChangedEventArgs("Item", default, obj));

        ExtendedPropertyChanged += handler;
    }

    public void Unlink(EventHandler<ExtendedPropertyChangedEventArgs> handler)
    {
        if (_target is IEnumerable e)
            foreach (var obj in e) handler(this, new ExtendedPropertyChangedEventArgs("Item", obj, null));

        ExtendedPropertyChanged -= handler;
    }

    public void TargetPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
        ExtendedPropertyChanged?.Invoke(sender, (args as ExtendedPropertyChangedEventArgs) ?? new ExtendedPropertyChangedEventArgs(args, default, default));
    }

    List<ITriggerEntry> _triggerEntries = new List<ITriggerEntry>();

    public ITriggerEntry BuildTrigger(EventHandler<ExtendedPropertyChangedEventArgs> handler)
    {
        var entry = new TriggerEntryCollection(this, handler);
        _triggerEntries.Add(entry);
        return entry;
    }
    //public ITriggerEntry GetTrigger(Action<object, ExtendedPropertyChangedEventArgs> handler)
    //{
    //    var entry = new TriggerEntryCollection(this, handler);
    //    _triggerEntries.Add(entry);
    //    return entry;
    //}

    public ITriggerEntry BuildTrigger(TriggerPath path, EventHandler<ExtendedPropertyChangedEventArgs> handler)
    {
        var entry = new TriggerEntryCollectionWithPath(this, path, handler);
        _triggerEntries.Add(entry);
        return entry;
    }

    public bool IsLinked() => ExtendedPropertyChanged != null;

    public void Dispose()
    {
        if (ExtendedPropertyChanged == null) return;
        foreach (var d in ExtendedPropertyChanged.GetInvocationList())
        {
            if (d is EventHandler<ExtendedPropertyChangedEventArgs> h)
                Unlink(h);
        }
    }

}