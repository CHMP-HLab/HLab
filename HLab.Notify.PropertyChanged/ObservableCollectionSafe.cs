using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using HLab.Base;
using HLab.DependencyInjection.Annotations;

namespace HLab.Notify.PropertyChanged
{
    public interface IChildObject
    {
        void SetParent(object parent,INotifyClassParser parser,Action<PropertyChangedEventArgs> args);
    }

    public class ObservableCollectionSafe<T> : N<ObservableCollectionSafe<T>>,
    IList<T>, IList, IReadOnlyList<T>, INotifyCollectionChanged, ILockable, IChildObject
    where T : class
    {
        //[Import]
        //public ObservableCollectionSafe(IEventHandlerService eventHandlerService) : base(eventHandlerService)
        //{
        //    RegisterTriggers();
        //}
        public ObservableCollectionSafe()
        {
            H.Initialize(this,OnPropertyChanged);
        }

        private readonly List<T> _list = new List<T>();

        //TODO : remove recursion
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly ConcurrentQueue<NotifyCollectionChangedEventArgs> _changedQueue = new ConcurrentQueue<NotifyCollectionChangedEventArgs>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public virtual T Selected
        {
            get => _selected.Get();
            set => _selected.Set(value);
        }
        private readonly IProperty<T> _selected = H.Property<T>();

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs arg)
        {
            NotifyHelper.EventHandlerService.Invoke(CollectionChanged, this, arg);
        }

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();



        public int Count
        {
            get => _count.Get();
            private set => _count.Set(value);
        }
        private readonly IProperty<int> _count = H.Property<int>();

        int IList.Add(object value)
        {
            int r = 0;
            int idx = 0;
            _lock.EnterWriteLock();
            try
            {
                idx = _list.Count;
                r = ((IList) _list).Add(value);
                _changedQueue.Enqueue(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, idx));
                return r;

            }
            finally
            {

                OnCollectionChanged();
            }
        }

        public virtual void Add(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                var idx = _list.Count;
                _list.Add(item);
                Count = _list.Count;
                _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, idx));
            }
            finally
            {
                if(_lock.IsWriteLockHeld) _lock.ExitWriteLock();
                OnCollectionChanged();
            }
        }

        public bool AddUnique(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                if (Contains(item)) return false;
                var idx = _list.Count;
                _list.Add(item);
                Count = _list.Count;
                _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, idx));
                return true;
            }
            finally
            {
                if(_lock.IsWriteLockHeld) _lock.ExitWriteLock();
                OnCollectionChanged();
            }
        }

        void IList.Remove(object value)
        {
            _lock.EnterWriteLock();
            try
            {
                var idx = ((IList)_list).IndexOf(value);
                ((IList)_list).Remove(value);
                Count = _list.Count;
                _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, idx));
            }
            finally
            {
                if(_lock.IsWriteLockHeld) _lock.ExitWriteLock();
                OnCollectionChanged();
            }
        }

        public bool Remove(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                var idx = _list.IndexOf(item);
                var r = _list.Remove(item);
                Count = _list.Count;
                _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, idx));
                return r;
            }
            finally
            {
                if(_lock.IsWriteLockHeld) _lock.ExitWriteLock();
                OnCollectionChanged();
            }
        }

        public void RemoveAt(int index)
        {
            _lock.EnterWriteLock();
            try
            {
                var value = _list[index];
                _list.RemoveAt(index);
                Count = _list.Count;
                _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
            }
            finally
            {
                if(_lock.IsWriteLockHeld) _lock.ExitWriteLock();
                OnCollectionChanged();
            }
        }

        void IList.Insert(int index, object value)
        {
            _lock.EnterWriteLock();
            try
            {
                ((IList)_list).Insert(index, value);
                Count = _list.Count;
                _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
            }
            finally
            {
                if(_lock.IsWriteLockHeld) _lock.ExitWriteLock();
                OnCollectionChanged();
            }
        }

        public virtual void Insert(int index, T item)
        {
            _lock.EnterWriteLock();
            try
            {
                Debug.Assert(index <= _list.Count);
                _list.Insert(index, item);
                Count = _list.Count;
                _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }
            finally
            {
                if(_lock.IsWriteLockHeld) _lock.ExitWriteLock();
                OnCollectionChanged();
            }
        }

        public void CopyTo(Array array, int index)
        {
            _lock.EnterReadLock();
            try
            {
                ((IList)_list).CopyTo(array, index);
            }
            finally
            {
                if(_lock.IsWriteLockHeld) _lock.ExitWriteLock();
            }

        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _lock.EnterReadLock();
            try
            {
                _list.CopyTo(array, arrayIndex);
                _changedQueue.Enqueue(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, array, arrayIndex));
            }
            finally
            {
                if(_lock.IsReadLockHeld) _lock.ExitReadLock();
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
                if(_lock.IsReadLockHeld) _lock.ExitReadLock();
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
                if(_lock.IsReadLockHeld) _lock.ExitReadLock();
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
                if(_lock.IsReadLockHeld) _lock.ExitReadLock();
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
                if(_lock.IsReadLockHeld) _lock.ExitReadLock();
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
                    if(_lock.IsReadLockHeld) _lock.ExitReadLock();
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
                    return ((ICollection)_list).IsSynchronized;
                }
                finally
                {
                    if(_lock.IsReadLockHeld) _lock.ExitReadLock();
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
                    return ((IList) _list).IsFixedSize;
                }
                finally
                {
                    if(_lock.IsReadLockHeld) _lock.EnterReadLock();
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
                    return ((ICollection<T>) _list).IsReadOnly;
                }
                finally
                {
                    if(_lock.IsReadLockHeld) _lock.ExitReadLock();
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
                    return ((IList) _list).IsReadOnly;
                }
                finally
                {
                    if(_lock.IsReadLockHeld) _lock.ExitReadLock();
                }
            }
        }

        public ReaderWriterLockSlim Lock => _lock;

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
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    ((IList) _list)[index] = value;
                    Count = _list.Count;
                    _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                        value, index));
                }
                finally
                {
                    _lock.ExitWriteLock();
                    OnCollectionChanged();
                }
            }
        }

        public T this[int index]
        {
            get {
                _lock.EnterReadLock();
                try
                {
                    return _list[index];
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set {
                _lock.EnterWriteLock();
                try
                {
                    _list[index] = value;
                    Count = _list.Count;
                    _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                        value, index));
                }
                finally
                {
                    if(_lock.IsWriteLockHeld) _lock.ExitWriteLock();
                    OnCollectionChanged();
                }
            }
        }
        private void OnCollectionChanged()
        {
            while (_changedQueue.TryDequeue(out var a))
                OnCollectionChanged(a);
        }

        public void SetParent(object parent,INotifyClassParser parser, Action<PropertyChangedEventArgs> action)
        {
            //TODO
        }
    }
}
