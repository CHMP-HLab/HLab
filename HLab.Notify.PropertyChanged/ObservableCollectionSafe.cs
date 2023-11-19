using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using HLab.Base;
using Nito.AsyncEx;

namespace HLab.Notify.PropertyChanged;

public class ObservableCollectionSafe<T> :// ChildObject,
    IList<T>, IList, IReadOnlyList<T>, INotifyCollectionChanged, ILockable
    where T : class
{
    readonly List<T> _list = new List<T>();

    readonly AsyncReaderWriterLock _lock = new AsyncReaderWriterLock();
    readonly ConcurrentQueue<NotifyCollectionChangedEventArgs> _collectionChangedQueue = new ConcurrentQueue<NotifyCollectionChangedEventArgs>();
    readonly ConcurrentQueue<PropertyChangedEventArgs> _propertyChangedQueue = new ConcurrentQueue<PropertyChangedEventArgs>();

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    //public virtual T Selected
    //{
    //    get => _selected.Get();
    //    set => _selected.Set(value);
    //}
    //private readonly IProperty<T> _selected = H.Property<T>();

    void OnCollectionChanged(NotifyCollectionChangedEventArgs arg)
    {
        NotifyHelper.EventHandlerService.Invoke(CollectionChanged, this, arg);
    }

    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();



    public int Count => _list.Count;


    void _enqueue(NotifyCollectionChangedEventArgs args) => _collectionChangedQueue.Enqueue(args);


    int IList.Add(object value) => DoWriteLocked(() =>
    {
        var idx = _list.Count;
        var r = ((IList) _list).Add(value);
        _enqueue(
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, idx));
        return r;
    });

    public virtual void Add(T item) => DoWriteLocked(() => _add(item));

    void _add(T item)
    {
        var idx = _list.Count;
        _list.Add(item);
        _enqueue(
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, idx));
    }

    public bool AddUnique(T item) => DoWriteLocked(() =>
    {
        if (Contains(item)) return false;
        var idx = _list.Count;
        _list.Add(item);
        _enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, idx));
        return true;
    });

    void IList.Remove(object value) => DoWriteLocked(() =>
    {
        var idx = ((IList) _list).IndexOf(value);
        ((IList) _list).Remove(value);
        _enqueue(
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, idx));
    });

    public bool Remove(T item) => DoWriteLocked(() =>
    {
        var idx = _list.IndexOf(item);
        var r = _list.Remove(item);
        _enqueue(
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, idx));
        return r;
    });

    public void RemoveAt(int index) => DoWriteLocked(() =>
    {
        var value = _list[index];
        _list.RemoveAt(index);
        _enqueue(
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
    });

    void IList.Insert(int index, object value) => DoWriteLocked(() =>
    {
        ((IList) _list).Insert(index, value);
        _enqueue(
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
    });

    public virtual void Insert(int index, T item) => DoWriteLocked(() =>
    {
        Debug.Assert(index <= _list.Count);
        _list.Insert(index, item);
        _enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
    });

    public void CopyTo(Array array, int index) => DoReadLocked(() =>
    {
        ((IList) _list).CopyTo(array, index);
    });

    public void CopyTo(T[] array, int arrayIndex) => DoReadLocked(() =>
    {
        _list.CopyTo(array, arrayIndex);
    });

    public void Clear() => DoWriteLocked(() =>
    { 
        _list.Clear();
        _enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

    });

    bool IList.Contains(object value) => DoReadLocked(() => ((IList) _list).Contains(value));

    public bool Contains(T item) => DoReadLocked(() => _list.Contains(item));

    int IList.IndexOf(object value) => DoReadLocked(() => ((IList)_list).IndexOf(value));

    public int IndexOf(T item) => DoReadLocked(() => _list.IndexOf(item));

    object ICollection.SyncRoot => DoReadLocked(() => ((ICollection)_list).SyncRoot);

    bool ICollection.IsSynchronized => DoReadLocked(() => ((ICollection)_list).IsSynchronized);

    bool IList.IsFixedSize => DoReadLocked(() => ((IList) _list).IsFixedSize);

    bool ICollection<T>.IsReadOnly => DoReadLocked(() => ((ICollection<T>) _list).IsReadOnly);

    bool IList.IsReadOnly => DoReadLocked(() => ((IList) _list).IsReadOnly);

    AsyncReaderWriterLock ILockable.Lock => _lock;

    object IList.this[int index]
    {
        get => DoReadLocked(()=>_list[index] ) ;
        set => DoWriteLocked(() =>
        {
            ((IList) _list)[index] = value;
            _enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                value, index));

        });
    }

    public T this[int index]
    {
        get => DoReadLocked(()=>_list[index] ) ;
        set => DoWriteLocked(() =>
        {
            _list[index] = value;
            _collectionChangedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                value, index));

        });
    }

    public T GetOrAdd(Func<T, bool> predicate, Func<T> create) => DoWriteLocked(() =>
    {
        var value = _list.FirstOrDefault(predicate);
        if (value == null)
        {
            value = create();
            _add(value);
        }
        return value;
    });

    public T AddOrUpdate(Func<T, bool> predicate, Action<T> update, Func<T> create) => DoWriteLocked(() =>
    {
        var value = _list.FirstOrDefault(predicate);
        if (value == null)
        {
            value = create();
            _add(value);
        }
        else
        {
            update?.Invoke(value);
        }
        return value;
    });

    TReturn DoReadLocked<TReturn>(Func<TReturn> action)
    {
        using (_lock.ReaderLock())
        {
            return action();
        }
    }

    void DoReadLocked(Action action)
    {
        using (_lock.ReaderLock())
        {
            action();
        }
    }

    TReturn DoWriteLocked<TReturn>(Func<TReturn> action)
    {
        try
        {
            using (_lock.WriterLock())
            {
                return action();
            }
        }
        finally
        {
            OnCollectionChanged();
        }
    }

    void DoWriteLocked(Action action)
    {
        try
        {
            using (_lock.WriterLock())
            {
                action();
            }
        }
        finally
        {
            OnCollectionChanged();
        }

    }


    //private int _oldCount = 0;
    void OnCollectionChanged()
    {
        while (_collectionChangedQueue.TryDequeue(out var a))
            OnCollectionChanged(a);
        //if(_list.Count!=_oldCount)
    }

    public void SetParent(object parent,INotifyClassHelper parser, Action<PropertyChangedEventArgs> action)
    {
        //TODO
    }

    //public ObservableCollectionSafe(ConfiguratorEntry configurator) : base(configurator)
    //{
    //}
}