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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

using HLab.Base;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using Nito.AsyncEx;

namespace HLab.Notify.Collections
{
    public class ObservableCollectionNotifier<T> : NotifierBase,
        IList<T>, IList, IReadOnlyList<T>, INotifyCollectionChanged, ILockable
        //where T : INotifyPropertyChanged
    {
        public ObservableCollectionNotifier(IEventHandlerService eventHandlerService)
        {
            _eventHandlerService = eventHandlerService??NotifyHelper.EventHandlerService;

            H.Initialize(this);
        }

        protected class H : H<ObservableCollectionNotifier<T>> { }

        #region Dependencies

        private readonly IEventHandlerService _eventHandlerService;

        #endregion

        private readonly List<T> _list = new List<T>();

        private readonly AsyncReaderWriterLock _lock = new AsyncReaderWriterLock();

        public event NotifyCollectionChangedEventHandler CollectionChanged;


        public virtual T Selected
        {
            get
            {
                using (_lock.ReaderLock())
                {
                    return _selected.Get();
                }
            }
            set
            {
                using (_lock.WriterLock())
                {
                    _selected.Set(value);
                }
            }
        }
        protected IProperty<T> _selected = H.Property<T>();

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs arg)
        {
             _eventHandlerService.Invoke(CollectionChanged,this,arg);
        }



        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

        public int Count => _count.Get();
        private IProperty<int> _count = H.Property<int>(c => c
            .Set(e => e._list.Count)
        );

        int IList.Add(object value)
        {
            int r = 0;
            int idx = 0;
            using (_lock.WriterLock())
            {
                idx = _list.Count;
                r = ((IList)_list).Add(value);
                _count.Set(_list.Count);
            }
             OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, idx));
             return r;
        }

        public virtual void Add(T item)
        {
            int r = 0;
            int idx = 0;
            using (_lock.WriterLock())
            {
                idx = _list.Count;
                _list.Add(item);
                _count.Set(_list.Count);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, idx));
        }
        public bool AddUnique(T item)
        {
             int r = 0;
            int idx = 0;
            using (_lock.WriterLock())
            {
                if (Contains(item)) return false;
                _list.Add(item);
                _count.Set(_list.Count);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, idx));
            return true;
        }

        void IList.Remove(object value)
        {
            int r = 0;
            int idx = 0;
            using (_lock.WriterLock())
            {
                idx = ((IList)_list).IndexOf(value);
                ((IList)_list).Remove(value);
                _count.Set(_list.Count);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, idx));
        }

        public bool Remove(T item)
        {
            bool r = false;
            int idx = 0;
            using (_lock.WriterLock())
            {
                idx = _list.IndexOf(item);
                r = _list.Remove(item);
                _count.Set(_list.Count);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, idx));
            return r;
        }

        public void RemoveAt(int index)
        {
            T value;
            using (_lock.WriterLock())
            {
                value = _list[index];
                _list.RemoveAt(index);
                _count.Set(_list.Count);
            }
            //OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value));
        }

        void IList.Insert(int index, object value)
        {
            using (_lock.WriterLock())
            {
                ((IList)_list).Insert(index, value);
                _count.Set(_list.Count);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
        }

        public virtual void Insert(int index, T item)
        {
            using (_lock.WriterLock())
            {
                Debug.Assert(index <= _list.Count);
                _list.Insert(index, item);
                _count.Set(_list.Count);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        public void CopyTo(Array array, int index)
        {
            using (_lock.WriterLock())
            {
                ((IList)_list).CopyTo(array, index);
                _count.Set(_list.Count);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, array, index));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            using (_lock.WriterLock())
            {
                _list.CopyTo(array, arrayIndex);
                _count.Set(_list.Count);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, array, arrayIndex));
        }

        public void Clear() => _list.Clear();

        bool IList.Contains(object value)
        {
            using (_lock.ReaderLock())
            {
                return ((IList)_list).Contains(value);
            }
        }

        public bool Contains(T item)
        {
            using (_lock.ReaderLock())
            {
                return _list.Contains(item);
            }
        }


        int IList.IndexOf(object value)
        {
            using (_lock.ReaderLock())
            {
                return ((IList)_list).IndexOf(value);
            }
        }

        public int IndexOf(T item)
        {
            using (_lock.ReaderLock())
            {
                return _list.IndexOf(item);
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                using (_lock.ReaderLock())
                {
                    return ((ICollection)_list).SyncRoot;
                } 
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                using (_lock.ReaderLock())
                {
                    return ((ICollection) _list).IsSynchronized;
                }
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                using (_lock.ReaderLock())
                {
                    return ((IList)_list).IsFixedSize;
                }
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                using (_lock.ReaderLock())
                {
                    return ((ICollection<T>)_list).IsReadOnly;
                }
           }
        }

        bool IList.IsReadOnly
        {
            get
            {
                using (_lock.ReaderLock())
                {
                    return ((IList)_list).IsReadOnly;
                }
            }
        }

        public AsyncReaderWriterLock Lock => _lock;

        object IList.this[int index]
        {
            get
            {
                using (_lock.ReaderLock())
                {
                    return _list[index];
                }
             }
            set
            {
                using (_lock.WriterLock())
                {
                    ((IList)_list)[index] = value;
                    _count.Set(_list.Count);
                }
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, index));
            }
        }

        public T this[int index]
        {
            get
            {
                using (_lock.ReaderLock())
                {
                    return _list[index];
                }
            }
            set
            {
                using (_lock.WriterLock())
                {
                    _list[index] = value;
                    _count.Set(_list.Count);
                }
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, index));
            }
        }
    }
}
