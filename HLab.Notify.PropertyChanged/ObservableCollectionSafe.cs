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
using Nito.AsyncEx;

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
        private readonly AsyncReaderWriterLock _lock = new AsyncReaderWriterLock();
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

        int IList.Add(object value) => DoWriteLocked(() =>
        {
            var idx = _list.Count;
            var r = ((IList) _list).Add(value);
            _changedQueue.Enqueue(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, idx));
            return r;
        });

        public virtual void Add(T item) => DoWriteLocked(() =>
        {
                var idx = _list.Count;
                _list.Add(item);
                Count = _list.Count;
                _changedQueue.Enqueue(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, idx));
        });

        public bool AddUnique(T item) => DoWriteLocked(() =>
        {
            if (Contains(item)) return false;
            var idx = _list.Count;
            _list.Add(item);
            Count = _list.Count;
            _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, idx));
            return true;
        });

        void IList.Remove(object value) => DoWriteLocked(() =>
        {
            var idx = ((IList) _list).IndexOf(value);
            ((IList) _list).Remove(value);
            Count = _list.Count;
            _changedQueue.Enqueue(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, idx));
        });

        public bool Remove(T item) => DoWriteLocked(() =>
        {
            var idx = _list.IndexOf(item);
            var r = _list.Remove(item);
            Count = _list.Count;
            _changedQueue.Enqueue(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, idx));
            return r;
        });

        public void RemoveAt(int index) => DoWriteLocked(() =>
        {
            var value = _list[index];
            _list.RemoveAt(index);
            Count = _list.Count;
            _changedQueue.Enqueue(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
        });

        void IList.Insert(int index, object value) => DoWriteLocked(() =>
        {
            ((IList) _list).Insert(index, value);
            Count = _list.Count;
            _changedQueue.Enqueue(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
        });

        public virtual void Insert(int index, T item) => DoWriteLocked(() =>
        {
            Debug.Assert(index <= _list.Count);
            _list.Insert(index, item);
            Count = _list.Count;
            _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        });

        public void CopyTo(Array array, int index) => DoWriteLocked(() =>
        {
                ((IList) _list).CopyTo(array, index);
            _changedQueue.Enqueue(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, array, index));
        });

        public void CopyTo(T[] array, int arrayIndex) => DoWriteLocked(() =>
        {
            _list.CopyTo(array, arrayIndex);
            _changedQueue.Enqueue(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, array, arrayIndex));
        });

        public void Clear() => DoWriteLocked(() =>
        { 
            _list.Clear();
            _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

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
                Count = _list.Count;
                _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                    value, index));

            });
        }

        public T this[int index]
        {
            get => DoReadLocked(()=>_list[index] ) ;
            set => DoWriteLocked(() =>
            {
                    _list[index] = value;
                    Count = _list.Count;
                    _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                        value, index));

            });
        }
        private RT DoReadLocked<RT>(Func<RT> action)
        {
            //try
            //{
            using (_lock.ReaderLock())
            {
                return action();
            }
            //}
            //finally
            //{
            //   OnCollectionChanged();
            //}
        }
        private RT DoWriteLocked<RT>(Func<RT> action)
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
        private void DoWriteLocked(Action action)
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
