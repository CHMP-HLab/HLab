/*
  HLab.Mvvm
  Copyright (c) 2021 Mathieu GRENET.  All right reserved.

  This file is part of HLab.Mvvm.

    HLab.Mvvm is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    HLab.Mvvm is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MouseControl.  If not, see <http://www.gnu.org/licenses/>.

	  mailto:mathieu@mgth.fr
	  http://www.mgth.fr
*/

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using HLab.Base;
using Nito.AsyncEx;

namespace HLab.Notify.PropertyChanged;

public abstract class ObservableCollectionNotifier<T> : NotifierBase,
    IList<T>, IList, IReadOnlyList<T>, INotifyCollectionChanged, ILockable
{
    protected ObservableCollectionNotifier()
    {
        H<ObservableCollectionNotifier<T>>.Initialize(this);
    }


    #region Dependencies

    #endregion

    readonly List<T> _list = new();

    protected AsyncReaderWriterLock Lock { get; } = new();

    readonly ConcurrentQueue<NotifyCollectionChangedEventArgs> _changedQueue = new();

    public event NotifyCollectionChangedEventHandler CollectionChanged;


    public virtual T Selected
    {
        get => DoReadLocked(() => _selected.Get());
        set => DoWriteLocked(()=>_selected.Set(value));
    }

    readonly IProperty<T> _selected = H<ObservableCollectionNotifier<T>>.Property<T>();




    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

    public int Count => _count.Get();

    readonly IProperty<int> _count = H<ObservableCollectionNotifier<T>>.Property<int>(c => c
        .Set(e => e._list.Count)
    );

    int IList.Add(object item) => _add((T)item);
    public virtual void Add(T item) => _add(item);

    int _add(T item)
    {
        try
        {
            using (Lock.WriterLock())
            {
                var index = _list.Count;
                _list.Add(item);
                _count.Set(_list.Count);
                _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
                return index;
            }
        }
        finally
        {
            OnCollectionChanged();
        }
    }

    public bool AddUnique(T item) => DoWriteLocked<bool>(() =>
    {
        if (Contains(item)) return false;
        var index = Count;
        _list.Add(item);
        _count.Set(_list.Count);
        _changedQueue.Enqueue(
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        return true;
    });


    public bool Remove(T item) => DoWriteLocked(() =>
        {
            var index = _list.IndexOf(item);
            var r = _list.Remove(item);
            _count.Set(_list.Count);
            _changedQueue.Enqueue(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            return r;
        }
    );




    protected void RemoveAtNoLock(int index)
    {
        var item = _list[index];
        _list.RemoveAt(index);
        _count.Set(_list.Count);
        _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
    }

    public void RemoveAt(int index) => DoWriteLocked(() => RemoveAtNoLock(index));



    public virtual void Insert(int index, T item) => DoWriteLocked(() => InsertNoLock(index, item));

    protected void InsertNoLock(int index, T item)
    {
        Debug.Assert(index <= _list.Count);
        _list.Insert(index, item);
        _count.Set(_list.Count);
        _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
    }



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
        _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    });

    bool IList.Contains(object value) => DoReadLocked(() => ((IList) _list).Contains(value));

    public bool Contains(T item) => DoReadLocked(() => _list.Contains(item));

    void IList.Insert(int index, object value) => Insert(index, (T)value);
    void IList.Remove(object item) => Remove((T) item);
    int IList.IndexOf(object value) => DoReadLocked(() => ((IList)_list).IndexOf(value));
    public int IndexOf(T item) => DoReadLocked(() => _list.IndexOf(item));
    object ICollection.SyncRoot => DoReadLocked(() => ((ICollection) _list).SyncRoot);
    bool ICollection.IsSynchronized => DoReadLocked(() => ((ICollection) _list).IsSynchronized);
    bool IList.IsFixedSize => DoReadLocked(() => ((IList)_list).IsFixedSize);
    bool ICollection<T>.IsReadOnly => DoReadLocked(() => ((ICollection<T>) _list).IsReadOnly);
    bool IList.IsReadOnly => DoReadLocked(() => ((IList)_list).IsReadOnly);

    object IList.this[int index]
    {
        get => DoReadLocked(()=>_list[index]);
        set => DoWriteLocked(() =>
            {
                ((IList)_list)[index] = value;
                _count.Set(_list.Count);
                _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, index));
            }
        );
    }

    protected T GetNoLock(int index) => _list[index];

    public T this[int index]
    {
        get => DoReadLocked(()=>_list[index]);
        set => DoWriteLocked(() =>
        {
            _list[index] = value;
            _count.Set(_list.Count);
            _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, index));
        });
    }

    protected void OnCollectionChanged()
    {
        while (_changedQueue.TryDequeue(out var a))
            OnCollectionChanged(a);
    }

    void OnCollectionChanged(NotifyCollectionChangedEventArgs arg)
    {
        CollectionChanged?.Invoke(this,arg);
    }

    AsyncReaderWriterLock ILockable.Lock => Lock;

    TR DoReadLocked<TR>(Func<TR> action)
    {
        using (Lock.ReaderLock())
        {
            return action();
        }
    }

    void DoReadLocked(Action action)
    {
        using (Lock.ReaderLock())
        {
            action();
        }
    }

    TR DoWriteLocked<TR>(Func<TR> action)
    {
        try
        {
            using (Lock.WriterLock())
            {
                using(ClassHelper.GetSuspender())
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
            using (Lock.WriterLock())
            {
                using(ClassHelper.GetSuspender())
                    action();
            }
        }
        finally
        {
            OnCollectionChanged();
        }
    }
}