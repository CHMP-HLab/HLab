using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HLab.Notify.Annotations;

internal class TriggerOnEntryCollection : TriggerOnEntry
{
    readonly ConditionalWeakTable<object,TriggerOnEntry> _next = new ConditionalWeakTable<object, TriggerOnEntry>();
    readonly INotifyCollectionChanged _collection;
    public TriggerOnEntryCollection(PropertyChangedEventHandler handler, INotifyCollectionChanged collection, TriggerPath path)
        :base(path,handler)
    {
        _collection = collection;

        if (path!=null)
        {
            _collection.CollectionChanged += OnCollectionChanged_Subscribe;
            OnCollectionChanged_Subscribe(collection,new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,(IList)_collection));
        }
        _collection.CollectionChanged += OnCollectionChanged_Handler;
    }

    void OnCollectionChanged_Handler(object sender, NotifyCollectionChangedEventArgs args)
    {
        Handler.Invoke(_collection, new NotifierPropertyChangedEventArgs("Item", args.OldItems, args.NewItems));
    }

    void OnCollectionChanged_Subscribe(object sender, NotifyCollectionChangedEventArgs args)
    {
        switch(args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (var item in args.NewItems)
                {
                    if (_next.TryGetValue(item, out var old))
                    {
                        //TODO : thread safe ?
                        old.Count++;
                    }
                    else
                        _next.Add(item,Subscribe(item));
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                foreach (var item in args.OldItems)
                {
                    if (_next.TryGetValue(item, out var t))
                    {
                        if (t.Count > 1)
                            t.Count--;
                        else
                        {
                            _next.Remove(item);
                            t.Dispose();                                    
                        }
                    }
                    else Debug.Assert(false);
                }
                break;
            case NotifyCollectionChangedAction.Replace:
                throw new NotImplementedException();
            //break;
            case NotifyCollectionChangedAction.Move:
                throw new NotImplementedException();
            //break;
            case NotifyCollectionChangedAction.Reset:
                //    throw new NotImplementedException();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}