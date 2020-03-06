﻿/*
  HLab.Mvvm
  Copyright (c) 2017 Mathieu GRENET.  All right reserved.

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
using System.Threading;
using HLab.Base;
using HLab.DependencyInjection.Annotations;

namespace HLab.Notify.PropertyChanged
{
    public abstract class ObservableCollectionNotifier<T> : NotifierBase,
        IList<T>, IList, IReadOnlyList<T>, INotifyCollectionChanged, ILockable
        //where T : INotifyPropertyChanged
    {
        public ObservableCollectionNotifier(bool init = true)
        {
            if (_eventHandlerService == null) _eventHandlerService = NotifyHelper.EventHandlerService;
            if(init) H.Initialize(this,OnPropertyChanged);
        }

        protected class H : NotifyHelper<ObservableCollectionNotifier<T>> { }

        #region Dependencies

        [Import]
        private readonly IEventHandlerService _eventHandlerService;

        #endregion

        private readonly List<T> _list = new List<T>();

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly ConcurrentQueue<NotifyCollectionChangedEventArgs> _changedQueue = new ConcurrentQueue<NotifyCollectionChangedEventArgs>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;


        public virtual T Selected
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _selected.Get();
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    _selected.Set(value);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }
        protected IProperty<T> _selected = H.Property<T>();

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs arg)
        {
            CollectionChanged?.Invoke(this,arg);
        }



        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

        public int Count => _count.Get();
        private IProperty<int> _count = H.Property<int>(c => c
            .Set(e => e._list.Count)
        );

        int IList.Add(object item) => _add((T)item);
        public virtual void Add(T item) => _add(item);
        private int _add(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                var index = _list.Count;
                _list.Add(item);
                _count.Set(_list.Count);
                _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
                return index;
            }
            finally
            {
                _lock.ExitWriteLock();
                OnCollectionChanged();
            }
        }

        public bool AddUnique(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                if (Contains(item)) return false;
                var index = Count;
                _list.Add(item);
                _count.Set(_list.Count);
                _changedQueue.Enqueue(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }
            finally
            {
                _lock.ExitWriteLock();
                OnCollectionChanged();
            }
            return true;
        }

        void IList.Remove(object item)
        {
            Remove((T) item);
        }

        public bool Remove(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                var index = _list.IndexOf(item);
                var r = _list.Remove(item);
                _count.Set(_list.Count);
                _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
                return r;
            }
            finally
            {
                _lock.ExitWriteLock();
                OnCollectionChanged();
            }
        }

        protected void RemoveAtNoLock(int index)
        {
                var item = _list[index];
                _list.RemoveAt(index);
                _count.Set(_list.Count);
                _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }

        public void RemoveAt(int index)
        {
            _lock.EnterWriteLock();
            try
            {
                RemoveAtNoLock(index);
            }
            finally
            {
                _lock.ExitWriteLock();
                OnCollectionChanged();
            }
        }

        void IList.Insert(int index, object value)
        {
            _lock.EnterWriteLock();
            try
            {
                ((IList)_list).Insert(index, value);
                _count.Set(_list.Count);
                _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
            }
            finally
            {
                if(_lock.IsWriteLockHeld) _lock.ExitWriteLock();
                OnCollectionChanged();
            }
        }

        protected void InsertNoLock(int index, T item)
        {
                Debug.Assert(index <= _list.Count);
                _list.Insert(index, item);
                _count.Set(_list.Count);
                _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        public virtual void Insert(int index, T item)
        {
            _lock.EnterWriteLock();
            try
            {
                InsertNoLock(index,item);
            }
            finally
            {
                if(_lock.IsWriteLockHeld) _lock.ExitWriteLock();
                OnCollectionChanged();
            }
        }

        public void CopyTo(Array array, int index)
        {
            _lock.EnterWriteLock();
            try
            {
                ((IList)_list).CopyTo(array, index);
                _count.Set(_list.Count);
                _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, array, index));
            }
            finally
            {
                if(_lock.IsWriteLockHeld) _lock.ExitWriteLock();
                OnCollectionChanged();
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _lock.EnterWriteLock();
            try
            {
                _list.CopyTo(array, arrayIndex);
                _count.Set(_list.Count);
                _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, array, arrayIndex));
            }
            finally
            {
                if(_lock.IsWriteLockHeld) _lock.ExitWriteLock();
                OnCollectionChanged();
            }
        }

        public void Clear()
        {
            _lock.EnterWriteLock();
            try
            {
                _list.Clear();
                _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
            finally
            {
                if(_lock.IsWriteLockHeld) _lock.ExitWriteLock();
                OnCollectionChanged();
            }
        }

        bool IList.Contains(object value)
        {
            _lock.EnterReadLock();
            try
            {
                return ((IList) _list).Contains(value);
            }
            finally
            {
                if (_lock.IsReadLockHeld) _lock.ExitReadLock();
            }
        }

        public bool Contains(T item)
        {
            _lock.EnterReadLock();
            try
            {
                return _list.Contains(item);
            }
            finally
            {
                if (_lock.IsReadLockHeld) _lock.ExitReadLock();
            }
        }


        int IList.IndexOf(object value)
        {
            _lock.EnterReadLock();
            try
            {
                return ((IList)_list).IndexOf(value);
            }
            finally
            {
                if (_lock.IsReadLockHeld) _lock.ExitReadLock();
            }
        }

        public int IndexOf(T item)
        {
            _lock.EnterReadLock();
            try
            {
                return _list.IndexOf(item);
            }
            finally
            {
                if (_lock.IsReadLockHeld) _lock.ExitReadLock();
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return ((ICollection)_list).SyncRoot;
                } 
                finally
                {
                    if (_lock.IsReadLockHeld) _lock.ExitReadLock();
                }
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return ((ICollection) _list).IsSynchronized;
                }
                finally
                {
                    if (_lock.IsReadLockHeld) _lock.ExitReadLock();
                }
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return ((IList)_list).IsFixedSize;
                }
                finally
                {
                    if (_lock.IsReadLockHeld) _lock.ExitReadLock();
                }
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return ((ICollection<T>)_list).IsReadOnly;
                }
                finally
                {
                    if (_lock.IsReadLockHeld) _lock.ExitReadLock();
                }
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return ((IList)_list).IsReadOnly;
                }
                finally
                {
                    if (_lock.IsReadLockHeld) _lock.ExitReadLock();
                }
            }
        }

        public ReaderWriterLockSlim  Lock => _lock;

        object IList.this[int index]
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _list[index];
                }
                finally
                {
                    if (_lock.IsReadLockHeld) _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    ((IList)_list)[index] = value;
                    _count.Set(_list.Count);
                    _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, index));
                }
                finally
                {
                    if(_lock.IsWriteLockHeld) _lock.ExitWriteLock();
                    OnCollectionChanged();
                }
            }
        }

        public T this[int index]
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _list[index];
                }
                finally
                {
                    if (_lock.IsReadLockHeld) _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    _list[index] = value;
                    _count.Set(_list.Count);
                    _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, index));
                }

                finally
                {
                    if(_lock.IsWriteLockHeld) _lock.ExitWriteLock();
                    OnCollectionChanged();
                }
            }
        }

        protected void OnCollectionChanged()
        {
                while (_changedQueue.TryDequeue(out var a))
                    OnCollectionChanged(a);
        }
    }
}
