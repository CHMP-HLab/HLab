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
            int r = 0;
            int idx = 0;
            using (_lock.Write)
            {
                idx = _list.Count;
                _list.Add(item);
                Count = _list.Count;
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, idx));
        }

        public bool AddUnique(T item)
        {
            int r = 0;
            int idx = 0;
            using (_lock.Write)
            {
                if (Contains(item)) return false;
                _list.Add(item);
                Count = _list.Count;
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, idx));
            return true;
        }

        void IList.Remove(object value)
        {
            int r = 0;
            int idx = 0;
            using (_lock.Write)
            {
                idx = ((IList)_list).IndexOf(value);
                ((IList)_list).Remove(value);
                Count = _list.Count;
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, idx));
        }

        public bool Remove(T item)
        {
            bool r = false;
            int idx = 0;
            using (_lock.Write)
            {
                idx = _list.IndexOf(item);
                r = _list.Remove(item);
                Count = _list.Count;
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, idx));
            return r;
        }

        public void RemoveAt(int index)
        {
            T value;
            using (_lock.Write)
            {
                value = _list[index];
                _list.RemoveAt(index);
                Count = _list.Count;
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
        }

        void IList.Insert(int index, object value)
        {
            using (_lock.Write)
            {
                ((IList)_list).Insert(index, value);
                Count = _list.Count;
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
        }

        public virtual void Insert(int index, T item)
        {
            using (_lock.Write)
            {
                Debug.Assert(index <= _list.Count);
                _list.Insert(index, item);
                Count = _list.Count;
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        public void CopyTo(Array array, int index)
        {
            using (_lock.Write)
            {
                ((IList)_list).CopyTo(array, index);
            }
            //OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, array, index));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            using (_lock.Write)
            {
                _list.CopyTo(array, arrayIndex);
            }
            //OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, array, arrayIndex));
        }

        public void Clear() => _list.Clear();

        bool IList.Contains(object value)
        {
            using (_lock.Read)
            {
                return ((IList)_list).Contains(value);
            }
        }

        public bool Contains(T item)
        {
            using (_lock.Read)
            {
                return _list.Contains(item);
            }
        }


        int IList.IndexOf(object value)
        {
            using (_lock.Read)
            {
                return ((IList)_list).IndexOf(value);
            }
        }

        public int IndexOf(T item)
        {
            using (_lock.Read)
            {
                return _list.IndexOf(item);
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                using (_lock.Read)
                {
                    return ((ICollection)_list).SyncRoot;
                }
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                using (_lock.Read)
                {
                    return ((ICollection)_list).IsSynchronized;
                }
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                using (_lock.Read)
                {
                    return ((IList)_list).IsFixedSize;
                }
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                using (_lock.Read)
                {
                    return ((ICollection<T>)_list).IsReadOnly;
                }
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                using (_lock.Read)
                {
                    return ((IList)_list).IsReadOnly;
                }
            }
        }

        public Locker Lock => _lock;

        object IList.this[int index]
        {
            get
            {
                using (_lock.Read)
                {
                    return _list[index];
                }
            }
            set
            {
                using (_lock.Write)
                {
                    ((IList)_list)[index] = value;
                    Count = _list.Count;
                }
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, index));
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
