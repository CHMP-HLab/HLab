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
using System.Linq;
using System.Threading;
using HLab.Base;
using HLab.Notify.PropertyChanged.PropertyHelpers;
using Nito.AsyncEx;

namespace HLab.Notify.PropertyChanged;

public interface IObservableFilter : ITriggerable, INotifyCollectionChanged, IChildObject
{
    void Link(Func<INotifyCollectionChanged> getter);

}

public interface IObservableFilter<T> : IList<T>, IObservableFilter
{
    void AddFilter(Func<T, bool> expr) => AddFilter(expr,0,null);
    void AddFilter(Func<T, bool> expr, int order) => AddFilter(expr,order,null);
    void AddFilter(Func<T, bool> expr, string name) => AddFilter(expr,0,name);
    void AddFilter(Func<T, bool> expr, int order, string name);
    void RemoveFilter(string name);
}


    
    
public class ObservableFilterPropertyHolder<T> : ChildObjectN<ObservableFilterPropertyHolder<T>>, ILockable, IReadOnlyList<T>, IList, IObservableFilter<T> 
{
    class Filter
    {
        public string Name { get; }
        public Func<T, bool> Expression { get; } = null;
        public int Order { get; }

        public Filter(string name, Func<T, bool> expression, int order = 0)
        {
            Name = name;
            Expression = expression;
            Order = order;
        }
    }

    readonly AsyncReaderWriterLock _lock = new AsyncReaderWriterLock();
    readonly AsyncReaderWriterLock _lockFilters = new AsyncReaderWriterLock();

    readonly List<Filter> _filters = new List<Filter>();

    readonly List<T> _list = new List<T>();

    public void AddFilter(Func<T, bool> expr, int order = 0, string name = null)
    {
        using(_lockFilters.WriterLock())
        {
            if (name != null) _removeFilter(name);
            _filters.Add(new Filter(name, expr, order));
        }
    }

    void _removeFilter(string name)
    {
        foreach (Filter f in _filters.Where(f => f.Name == name).ToList())
        {
            _filters.Remove(f);
        }
    }

    public void RemoveFilter(string name)
    {
        using(_lockFilters.WriterLock())
        {
            _removeFilter(name);
        }
    }


    public class CreateHelper : IDisposable
    {
        public ObservableFilterPropertyHolder<T> List = null;
        public T ViewModel = default;

        public TVm GetViewModel<TVm>() where TVm : class => ViewModel as TVm;

        public bool Done = true;

        public void Dispose()
        {
        }
    }

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    void OnCollectionChanged(NotifyCollectionChangedEventArgs arg)
    {
        NotifyHelper.EventHandlerService.Invoke(CollectionChanged, this, arg);
    }

    Func<INotifyCollectionChanged> _listGetter = null;
    INotifyCollectionChanged _currentList = null;

    public AsyncReaderWriterLock Lock => _lock;

    public int Count => _list.Count;

    public bool IsReadOnly => true;
    public bool IsSynchronized => false;
    public object SyncRoot => throw new NotImplementedException();
    public bool IsFixedSize => false;

    INotifyCollectionChanged GetList()
    {
        try
        {
            var list = _listGetter();
            if (!ReferenceEquals(_currentList, list))
            {
                if (_currentList != null) _currentList.CollectionChanged -= _list_CollectionChanged;
                if (list != null) list.CollectionChanged += _list_CollectionChanged;
                _currentList = list;
                OnTriggered();
            }
            return _currentList;

        }
        catch(Exception)
        {
            return null;
        }
    }


    public void Link(Func<INotifyCollectionChanged> getter)
    {
        _listGetter = getter;
        GetList();
    }


    void _list_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                Debug.Assert(e.NewItems != null);
            {
                DoWriteLocked(() =>
                {
                    foreach (var item in e.NewItems.OfType<T>())
                    {
                        if (Match(item))
                        {
                            _add(item);
                        }
                    }
                });
            }
                break;

            case NotifyCollectionChangedAction.Remove:

                Debug.Assert(e.OldItems != null);
            {
                DoWriteLocked(() =>
                {
                    foreach (var item in e.OldItems.OfType<T>())
                    {
                        _remove(item);
                    }
                });
            }
                break;

            case NotifyCollectionChangedAction.Replace:
                throw new NotImplementedException("Replace not implemented");
            case NotifyCollectionChangedAction.Move:
                throw new NotImplementedException("Move not implemented");
            case NotifyCollectionChangedAction.Reset:
                //                       base.Clear();
                ;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    bool Match(T item)
    {
        using(_lockFilters.ReaderLock())
        {
            if (_filters == null) return true;
            return item != null && _filters.Where(filter => filter.Expression != null).All(filter => filter.Expression(item));
        }
    }


    int _trigging = 0;
    public void OnTriggered()
    {
        var c = Interlocked.Add(ref _trigging,1);

        if (c > 1) return;


        var list = GetList();

        if (list is ITriggerable t)
        {
            t.OnTriggered();
        }

        if (list is IEnumerable<T> l)
            //using ((l as ILockable)?.Lock.Read)
        {
            DoWriteLocked(() =>
            {
                foreach (var item in l)
                {
                    if (Match(item))
                        _add(item);
                    else
                        _remove(item);
                }
            });
        }

        var c1 = Interlocked.Exchange(ref _trigging, 0);
        if (c1>1) OnTriggered();
    }

    readonly ConcurrentStack<NotifyCollectionChangedEventArgs> _notify = new ConcurrentStack<NotifyCollectionChangedEventArgs>();

    void Notify()
    {
        var changed = false;

        if(_notify.Count > 1)
        { }

        while(!_notify.IsEmpty)
        {
            if(_notify.TryPop(out var args))
            {
                CollectionChanged?.Invoke(this, args);
            }

            changed = true;
        }
        if (changed)
        { 
            // Todo : count may not have changed
            OnPropertyChanged("Count");
            OnPropertyChanged("Item");
        }
    }

    void _add(T item)
    {
        //Todo : try remove this condition
        if (_list.Contains(item)) return;
        _list.Add(item);
        _notify.Push(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new T[] {item}));
    }

    void _remove(T item)
    {
        if (!_list.Contains(item)) return;
        _list.Remove(item);
        _notify.Push(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new T[] { item }));
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
            Notify();
        }
    }

    public void Add(T item)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public bool Contains(T item)
    {
        using(_lock.ReaderLock())
        {
            return _list.Contains(item);
        }
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        foreach (T value in this)
        {
            array.SetValue(value, arrayIndex);
            arrayIndex = arrayIndex + 1;
        }
    }

    public bool Remove(T item)
    {
        throw new NotImplementedException();
    }

    class FilterEnumerator : IEnumerator<T>, IDisposable
    {
        int _currentIdx;
        readonly ObservableFilterPropertyHolder<T> _filter;
        IDisposable _locker;
        internal FilterEnumerator(ObservableFilterPropertyHolder<T> filter)
        {
            _locker = filter.Lock.ReaderLock();
            _filter = filter;
            _currentIdx = -1;
        }

        public T Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            _locker?.Dispose();
            _locker = null;
        }

        public bool MoveNext()
        {
            if (++_currentIdx >= _filter._list.Count)
                return false;

            Current = _filter._list[_currentIdx];
            return true;
        }

        public void Reset()
        {
            _currentIdx = 0;
        }
    }

    public IEnumerator<T> GetEnumerator() => new FilterEnumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    object IList.this[int index] {
        get => this[index];
        set => this[index] = (T)value;
    }
    public T this[int index]
    {
        get
        {
            using(_lock.ReaderLock())
            {
                return _list[index];
            }
        }
        set
        {
            throw new NotImplementedException();
        }
    }

    public int IndexOf(T item)
    {
        using(_lock.ReaderLock())
        {
            return _list.IndexOf(item);
        }
    }

    public void Insert(int index, T item)
    {
        throw new NotImplementedException();
    }

    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }

    public void CopyTo(Array array, int index)
    {
        throw new NotImplementedException();
    }

    public int Add(object value)
    {
        throw new NotImplementedException();
    }

    public bool Contains(object value) => this.Contains((T)value);

    public int IndexOf(object value) => this.IndexOf((T) value);

    public void Insert(int index, object value)
    {
        throw new NotImplementedException();
    }

    public void Remove(object value)
    {
        throw new NotImplementedException();
    }

    public ObservableFilterPropertyHolder(PropertyActivator activator) : base(activator)
    {
    }
}

//public class ObservableFilter<TCLass,T> : N<ObservableFilter<TCLass,T>>, ILockable,
//    IReadOnlyList<T>,
//    IList,
//    IObservableFilter<T> 
//    where TCLass : class
//{
//    private class Filter
//    {
//        public string Name { get; set; }
//        public Func<T, bool> Expression { get; set; } = null;
//        public int Order { get; set; }
//    }
//    private readonly AsyncReaderWriterLock _lock = new AsyncReaderWriterLock();
//    private readonly AsyncReaderWriterLock _lockFilters = new AsyncReaderWriterLock();
//    private readonly List<Filter> _filters = new List<Filter>();
//    private readonly List<T> _list = new List<T>();

//    public IObservableFilter<T> AddFilter(Func<T, bool> expr, int order = 0, string name = null)
//    {
//        using(_lockFilters.WriterLock())
//        {
//            if (name != null) _removeFilter(name);
//            _filters.Add(new Filter
//            {
//                Name = name,
//                Expression = expr,
//                Order = order,
//            });
//            return this;
//        }
//    }

//    private void _removeFilter(string name)
//    {
//        foreach (Filter f in _filters.Where(f => f.Name == name).ToList())
//        {
//            _filters.Remove(f);
//        }
//    }

//    public ObservableFilter<TCLass,T> RemoveFilter(string name)
//    {
//        using(_lockFilters.WriterLock())
//        {
//            _removeFilter(name);
//            return this;
//        }
//    }


//    public class CreateHelper : IDisposable
//    {
//        public ObservableFilter<T> List = null;
//        public T ViewModel = default(T);

//        public TVm GetViewModel<TVm>() where TVm : class => ViewModel as TVm;

//        public bool Done = true;

//        public void Dispose()
//        {
//        }
//    }

//    public event NotifyCollectionChangedEventHandler CollectionChanged;
//    private void OnCollectionChanged(NotifyCollectionChangedEventArgs arg)
//    {
//        NotifyHelper.EventHandlerService.Invoke(CollectionChanged, this, arg);
//    }

//    private Func<INotifyCollectionChanged> _listGetter = null;
//    private INotifyCollectionChanged _currentList = null;

//    public AsyncReaderWriterLock Lock => _lock;

//    public int Count
//    {
//        get => _count.Get();
//        private set => _count.Set(value);
//    }
//    private readonly IProperty<int> _count = H.Property<int>();


//    public bool IsReadOnly => true;
//    public bool IsSynchronized => false;
//    public object SyncRoot => throw new NotImplementedException();
//    public bool IsFixedSize => false;

//    private INotifyCollectionChanged GetList()
//    {
//        Debug.Assert(_listGetter!=null);

//        var list = _listGetter();
//        if (!ReferenceEquals(_currentList, list))
//        {
//            if (_currentList != null) _currentList.CollectionChanged -= _list_CollectionChanged;
//            if (list != null) list.CollectionChanged += _list_CollectionChanged;
//            _currentList = list;
//            OnTriggered();
//        }
//        return _currentList;
//    }


//    public IObservableFilter<T> Link(Func<INotifyCollectionChanged> getter)
//    {
//        _listGetter = getter;
//        //GetList();
//        return this;
//    }


//    private void _list_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
//    {
//        switch (e.Action)
//        {
//            case NotifyCollectionChangedAction.Add:
//                Debug.Assert(e.NewItems != null);
//                {
//                    foreach (var item in e.NewItems.OfType<T>())
//                    {
//                        if (Match(item))
//                        {
//                            _add(item);
//                        }
//                    }
//                }
//                break;

//            case NotifyCollectionChangedAction.Remove:

//                Debug.Assert(e.OldItems != null);
//                {
//                    foreach (var item in e.OldItems.OfType<T>())
//                    {
//                        _remove(item);
//                    }
//                }
//                break;

//            case NotifyCollectionChangedAction.Replace:
//                throw new NotImplementedException("Replace not implemented");
//            case NotifyCollectionChangedAction.Move:
//                throw new NotImplementedException("Move not implemented");
//            case NotifyCollectionChangedAction.Reset:
//                //                       base.Clear();
//                ;
//                break;
//            default:
//                throw new ArgumentOutOfRangeException();
//        }
//    }

//    private bool Match(T item)
//    {
//        using(_lockFilters.WriterLock())
//        {
//            if (_filters == null) return true;
//            return item != null && _filters.Where(filter => filter.Expression != null)
//                       .All(filter => filter.Expression(item));
//        }
//    }


//    private int _trigging = 0;
//    public void OnTriggered()
//    {
//        var c = Interlocked.Add(ref _trigging, 1);

//        if (c > 1) return;


//        var list = GetList();

//        if (list is ITriggerable triggable)
//        {
//            triggable.OnTriggered();
//        }

//        if (list is IEnumerable<T> l)
//        {
//            List<T> list2;

//            //using ((l as ILockable)?.Lock.ReaderLock())
//            {
//                list2 = l.ToList();
//            }
//            foreach (var item in list2)
//            {
//                if (Match(item))
//                    _add(item);
//                else
//                    _remove(item);
//            }
                
//        }


//        var c1 = Interlocked.Exchange(ref _trigging, 0);
//        if (c1 > 1) OnTriggered();
//    }

//    private readonly ConcurrentStack<NotifyCollectionChangedEventArgs> _notify = new ConcurrentStack<NotifyCollectionChangedEventArgs>();


//    private Action<object,IObservableFilter<T>> _configurator;

//    public ObservableFilter(Action<object,IObservableFilter<T>> configurator) : base(false)
//    {
//        _configurator = configurator;
//        H.Initialize(this,OnPropertyChanged);
//    }
//    public void SetParent(object parent, INotifyClassParser parser, Action<PropertyChangedEventArgs> action)
//    {
//        if (parent is TCLass c)
//        {
//            _configurator(c, this);
//            //OnTriggered();
//        }
//    }

//    private void Notify()
//    {
//        while (!_notify.IsEmpty)
//            if (_notify.TryPop(out var args))
//            {
//                CollectionChanged?.Invoke(this, args);
//            }
//    }


//    private void _add(T item)
//    {
//        using(_lock.WriterLock())
//        {
//            if (_list.Contains(item)) return;
//            _list.Add(item);
//            _notify.Push(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new T[] {item}));
//            Count = _list.Count;
//        }
//        Notify();
//        OnPropertyChanged("Item");
//    }
//    private void _remove(T item)
//    {
//        using(_lock.WriterLock())
//        {
//            if (!_list.Contains(item)) return;
//            _list.Remove(item);
//            _notify.Push(
//                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new T[] {item}));
//            Count = _list.Count;
//        }
//        Notify();
//        OnPropertyChanged("Item");
//    }

//    public void Add(T item)
//    {
//        throw new NotImplementedException();
//    }

//    public void Clear()
//    {
//        throw new NotImplementedException();
//    }

//    public bool Contains(T item)
//    {
//        using(_lock.ReaderLock())
//        {
//            return _list.Contains(item);
//        }
//    }

//    public void CopyTo(T[] array, int arrayIndex)
//    {
//        using(_lock.ReaderLock())
//        {
//            foreach (T value in this)
//            {
//                array.SetValue(value, arrayIndex);
//                arrayIndex = arrayIndex + 1;
//            }
//        }
//    }

//    public bool Remove(T item)
//    {
//        throw new NotImplementedException();
//    }

//    class FilterEnumerator : IEnumerator<T>, IDisposable
//    {
//        private int _currentIdx;
//        private readonly ObservableFilter<TCLass,T> _filter;
//        private IDisposable _locker;
//        internal FilterEnumerator(ObservableFilter<TCLass,T> filter)
//        {
//            _locker =  filter.Lock.ReaderLock();
//            _filter = filter;
//            _currentIdx = -1;
//        }

//        public T Current { get; private set; }

//        object IEnumerator.Current => Current;

//        public void Dispose()
//        {
//            _locker?.Dispose();
//            _locker = null;
//        }

//        public bool MoveNext()
//        {
//            if (++_currentIdx >= _filter._list.Count)
//                return false;
//            else
//            {
//                Current = _filter._list[_currentIdx];
//                return true;
//            }
//        }

//        public void Reset()
//        {
//            _currentIdx = 0;
//        }
//    }

//    public IEnumerator<T> GetEnumerator() => new FilterEnumerator(this);

//    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

//    object IList.this[int index]
//    {
//        get => this[index];
//        set => this[index] = (T)value;
//    }
//    public T this[int index]
//    {
//        get
//        {
//            using (_lock.ReaderLock())
//            {
//                return _list[index];
//            }
//        }
//        set
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public int IndexOf(T item)
//    {
//        using (_lock.ReaderLock())
//        {
//            return _list.IndexOf(item);
//        }
//    }

//    public void Insert(int index, T item)
//    {
//        throw new NotImplementedException();
//    }

//    public void RemoveAt(int index)
//    {
//        throw new NotImplementedException();
//    }

//    public void CopyTo(Array array, int index)
//    {
//        throw new NotImplementedException();
//    }

//    public int Add(object value)
//    {
//        throw new NotImplementedException();
//    }

//    public bool Contains(object value) => this.Contains((T)value);

//    public int IndexOf(object value) => this.IndexOf((T)value);

//    public void Insert(int index, object value)
//    {
//        throw new NotImplementedException();
//    }

//    public void Remove(object value)
//    {
//        throw new NotImplementedException();
//    }

//}