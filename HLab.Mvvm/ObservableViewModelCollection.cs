﻿/*
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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using HLab.Base.Extensions;
using HLab.Mvvm.Annotations;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Mvvm
{
    [Obsolete]
    public class ObservableViewModelCollection<T> : ObservableCollection<T> , INotifyPropertyChangedWithHelper/*, ITriggable*/
        where T : INotifyPropertyChanged
    {
        private readonly ConditionalWeakTable<object, object> _weakTable= new ConditionalWeakTable<object, object>();
        private Func<object,T> _viewModelCreator = null;
        private Action<CreateHelper> _viewModelDestructor = null;

        private Func<IMvvmContext> _getMvvmContext = null;

        public ObservableViewModelCollection<T> AddCreator(Func<object, T> c)
        {
            _viewModelCreator = c;
            //if (_viewModelCreator == null) return this;
            //foreach (var vm in this)
            //{
            //    _viewModelCreator.Invoke(new CreateHelper { List = this, ViewModel = vm });
            //}
            return this;
        }
        public ObservableViewModelCollection<T> AddDestructor(Action<CreateHelper> d)
        {
            _viewModelDestructor = d;
            return this;
        }

        public class CreateHelper : IDisposable
        {
            public ObservableViewModelCollection<T> List = null;
            public T ViewModel = default(T);

            public TVm GetViewModel<TVm>() where TVm:class, INotifyPropertyChanged => ViewModel as TVm;

            public bool Done = true;

            public void Dispose()
            {
            }
        }

        private void SetList()
        {
            var list = _getList();
            if (!ReferenceEquals(list,_list))
            {
                //TODO : compare old list with new list and send CollectionChanged for diff

                if(_list!=null) _list.CollectionChanged -= _list_CollectionChanged;
                if (_list is INotifyPropertyChanged oldList)
                {
                    oldList.PropertyChanged -= L_PropertyChanged;
                }

                _list = _getList();

                if (_list == null) return;

                _list.SetObserver(_list_CollectionChanged);

                if ( _list is INotifyPropertyChanged l)
                {
                    l.PropertyChanged += L_PropertyChanged;
                }
            }
        }

        private Func<INotifyCollectionChanged> _getList = null;
        private INotifyCollectionChanged _list = null;
        public ObservableViewModelCollection<T> Link(Func<INotifyCollectionChanged> getList)
        {
            Debug.Assert(Count == 0);
            _getList = getList;
            SetList();

            return this;
        }

        // TODO
        private void L_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName != "Selected") return;
            //if (!(_list is ObservableCollectionNotifier<T> l)) return;


            //if (l.Selected == null)
            //{
            //    Selected = default(T);
            //}
            //else if (_weakTable.TryGetValue(l.Selected, out var vm))
            //{
            //    Selected = (T) vm;
            //}
        }

        public T Selected
        {
            get => _selected.Get();
            set {
                if (_selected.Set(Contains(value) ? value : default(T)))
                {
                    //TODO
                    //if (_list is ObservableCollectionNotifier<T> l) l.Selected = (T)(value as IViewModel)?.GetModel();
                }
            }
        }
        private IProperty<T> _selected = H<ObservableViewModelCollection<T>>.Property<T>();

        public bool Select(T entity)
        {
            var vm = this.FirstOrDefault(e => (e as INotifyPropertyChanged).Equals(entity));
            if (vm == null) return false;
            Selected = vm;
            return true;
        }

        public ObservableViewModelCollection<T> SetViewMode(Type viewMode, Type viewClass)
        {
            return AddCreator(e =>
            {
                // TODO : inplement viewClass
                var vm = (T)_getMvvmContext().GetLinked(e, viewMode, viewClass);
                if (vm == null) throw new ArgumentException(e.GetType().Name + " - " + viewMode.Name + "," + viewClass.Name);
                return vm;
            });
        }

        public ObservableViewModelCollection<T> SetViewMode<TViewMode>() => SetViewMode(typeof(TViewMode), typeof(IViewClassDefault));
        public ObservableViewModelCollection<T> SetViewMode<TViewMode, TViewClass>() => SetViewMode(typeof(TViewMode), typeof(TViewClass));

        public ObservableViewModelCollection<T> SetViewModeContext(Func<IMvvmContext> getContext)
        {
            _getMvvmContext = getContext;
            return this;
        }

        public IMvvmContext MvvmContext => _getMvvmContext();

        //public void Update()
        //{
        //    (_list as ObservableCollectionNotifier<T>)?.Update();
        //}


        private void _list_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Debug.Assert(e.NewItems != null);
                    Debug.Assert(e.NewItems.Count == (_list as IList)?.Count - Count);
                    {
                        int iNew = e.NewStartingIndex;
                        foreach (var item in e.NewItems)
                        {
                            if (item is INotifyPropertyChanged entity)
                            {
                                var vm = _viewModelCreator.Invoke(entity);
                                _weakTable.Add(entity, vm);
                                Insert(iNew, (T)vm);
                            }
                            iNew++;
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:

                    Debug.Assert(e.OldItems!=null);
                    {
                        int iOld = e.OldStartingIndex;
                        foreach (var item in e.OldItems)
                        {
                            if (!_weakTable.TryGetValue(item, out object vm)) continue;

                            if (ReferenceEquals(vm, Selected)) Selected = default(T);


                            _weakTable.Remove(item);
                            _viewModelDestructor?.Invoke(new CreateHelper { List = this, ViewModel = (T)vm });
                            Remove((T)vm);
                        }                    
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

            Debug.Assert(Count == (_list as IList)?.Count);
        }

        public void OnTriggered()
        {
            SetList();
        }

        public INotifyClassHelper ClassHelper { get; }
    }
}
