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
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm
{

    public class ViewModelCache
    {

        private class LinkedViewModels
        {
            private readonly ConcurrentDictionary<Type, object> _linked = new ConcurrentDictionary<Type, object>();

            public object GetOrAdd(Type type, Func<Type,object> factory)
            {
                return _linked.GetOrAdd(type, factory);
            }
        }


//        private readonly ConditionalWeakTable<object, object> _linked = new ConditionalWeakTable<object, object>();
        //private readonly ConcurrentDictionary<Type, ConcurrentQueue<Action<object>>> _creators;


        private readonly ConditionalWeakTable<object, LinkedViewModels> _linked = new ConditionalWeakTable<object, LinkedViewModels>();

        //private readonly Type _viewMode;
        private readonly IMvvmContext _context;
        private readonly IMvvmService _mvvm;

        public ViewModelCache(IMvvmContext context, IMvvmService mvvm)
        {
            _context = context;
            _mvvm = mvvm;
        }

        //public object GetLinked(object baseObject, Type viewClass)
        //{
        //    var context = _context;
        //    return GetLinked(baseObject, viewClass, ref context);
        //}

        public object GetLinked(object baseObject,Type viewMode, Type viewClass)
        {
            if(baseObject==null) return null;

            var context = _context;
            { 
                if (baseObject is IViewModel vm)
                {
                    // if the viewModel was created outside, it may not contain a context
                    if(vm.MvvmContext==null)
                    {
                        if (vm is IMvvmContextProvider p)
                        {
                            context = context.GetChildContext(p.GetType().Name);
                            p.ConfigureMvvmContext(context);
                            vm.MvvmContext = context;
                        }
                        vm.MvvmContext = context;
                    }
                    else
                        // set current context to be the view model one
                        context = vm.MvvmContext;
                }
            }
            var linkedType = _mvvm.GetLinkedType(baseObject.GetType(), viewMode, viewClass);

            if (linkedType == null)
            {
                return null;
            }

            // we don't want to cache views cause they cannot be used twice
            if (baseObject == null || typeof(IView).IsAssignableFrom(linkedType))
            {
                //linkedObject is View
                return context.Locate(linkedType, baseObject);
            }
            else
            {
                // baseObject is ViewModel
                var cache = _linked.GetOrCreateValue(baseObject);
                return cache.GetOrAdd(linkedType, (t) => context.Locate(t, baseObject));
            }
        }

        public IView GetView(object baseObject,Type viewMode, Type viewClass)
        {

            if (viewClass == null) viewClass = typeof(IViewClassDefault);
            if (viewMode == null) viewMode = typeof(ViewModeDefault);

            while (true)
            {
                var linked = GetLinked(baseObject,viewMode,viewClass);

                if (linked == null)
                {
                    return _mvvm.GetNotFoundView(baseObject?.GetType(),viewMode,viewClass);
                }

                if (linked is IView fe)
                {
                    _mvvm.PrepareView(fe);
                    return fe;
                }

                baseObject = linked;
            }
        }



        // TODO 
        //public event DependencyPropertyChangedEventHandler ViewDataContextChanged;
        //private void View_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    ViewDataContextChanged?.Invoke(sender, e);
        //}
    }
}
