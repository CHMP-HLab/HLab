/*
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
using Nito.AsyncEx;

namespace HLab.Notify.PropertyChanged
{
    public abstract class ObservableCollectionNotifier<T> : NotifierBase,
        IList<T>, IList, IReadOnlyList<T>, INotifyCollectionChanged, ILockable
        //where T : INotifyPropertyChanged
    {
        public ObservableCollectionNotifier(bool init = true)
        {
            if (_eventHandlerService == null) _eventHandlerService = NotifyHelper.EventHandlerService;
            if(init) H<ObservableCollectionNotifier<T>>.Initialize(this);
        }


        #region Dependencies

        [Import]
        private readonly IEventHandlerService _eventHandlerService;

        #endregion

        private readonly List<T> _list = new List<T>();

        private readonly AsyncReaderWriterLock _lock = new AsyncReaderWriterLock();
        protected AsyncReaderWriterLock Lock => _lock;

        private readonly ConcurrentQueue<NotifyCollectionChangedEventArgs> _changedQueue = new ConcurrentQueue<NotifyCollectionChangedEventArgs>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;


        public virtual T Selected
        {
            get
            {
                using(Lock.ReaderLock())
                {
                    return _selected.Get();
                }
            }
            set
            {
                using(Lock.WriterLock())
                {
                    _selected.Set(value);
                }
            }
        }
        protected IProperty<T> _selected = H<ObservableCollectionNotifier<T>>.Property<T>();

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs arg)
        {
            CollectionChanged?.Invoke(this,arg);
        }



        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

        public int Count => _count.Get();
        private IProperty<int> _count = H<ObservableCollectionNotifier<T>>.Property<int>(c => c
            .Set(e => e._list.Count)
        );

        int IList.Add(object item) => _add((T)item);
        public virtual void Add(T item) => _add(item);
        private int _add(T item)
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

        public bool AddUnique(T item)
        {
            try
            {
                using (Lock.WriterLock())
                {
                    if (Contains(item)) return false;
                    var index = Count;
                    _list.Add(item);
                    _count.Set(_list.Count);
                    _changedQueue.Enqueue(
                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
                }
            }
            finally
            {
                OnCollectionChanged();
            }
            return true;
        }

        void IList.Remove(object item)
        {
            Remove((T) item);
        }

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



        void IList.Insert(int index, object value) => Insert(index, (T)value);
        public virtual void Insert(int index, T item) => DoWriteLocked(() => InsertNoLock(index, item));

        protected void InsertNoLock(int index, T item)
        {
                Debug.Assert(index <= _list.Count);
                _list.Insert(index, item);
                _count.Set(_list.Count);
                _changedQueue.Enqueue(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }



        public void CopyTo(Array array, int index) => DoWriteLocked(() =>
        {
            ((IList) _list).CopyTo(array, index);
            _count.Set(_list.Count);
            _changedQueue.Enqueue(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, array, index));
        });

        public void CopyTo(T[] array, int arrayIndex) => DoWriteLocked(() =>
        {
            _list.CopyTo(array, arrayIndex);
            _count.Set(_list.Count);
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

        AsyncReaderWriterLock ILockable.Lock => Lock;
        private RT DoReadLocked<RT>(Func<RT> action)
        {
            //try
            //{
            using (Lock.ReaderLock())
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
                using (Lock.WriterLock())
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
                using (Lock.WriterLock())
                {
                    action();
                }
            }
            finally
            {
                OnCollectionChanged();
            }

        }
    }
}
