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
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm;

public class ViewModelCache
{
    class LinkedViewModels
    {
        readonly ConcurrentDictionary<Type, object> _linked = new();

        public object GetOrAdd(Type type, Func<Type,object> factory)
        {
            return _linked.GetOrAdd(type, factory);
        }
    }


//        private readonly ConditionalWeakTable<object, object> _linked = new ConditionalWeakTable<object, object>();
    //private readonly ConcurrentDictionary<Type, ConcurrentQueue<Action<object>>> _creators;


    readonly ConditionalWeakTable<object, LinkedViewModels> _linked = new();

    //private readonly Type _viewMode;
    readonly IMvvmContext _context;
    readonly IMvvmService _mvvm;

    public ViewModelCache(IMvvmContext context, IMvvmService mvvm)
    {
        _context = context;
        _mvvm = mvvm;
    }

    public async Task<object?> GetLinkedAsync(object? baseObject,Type viewMode, Type viewClass, CancellationToken token = default)
    {
        //We cannot retrieve a view for a null object
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
        var linkedType = await _mvvm.GetLinkedTypeAsync(baseObject.GetType(), viewMode, viewClass, token);

        if (linkedType == null)
        {
            return null;
        }

        // we don't want to cache views cause they cannot be used twice
        if (true || typeof(IView).IsAssignableFrom(linkedType))
        {
            //linkedObject is View
            return context.Locate(linkedType, baseObject);
        }

        // baseObject is ViewModel
        var cache = _linked.GetOrCreateValue(baseObject);
        return cache.GetOrAdd(linkedType, (t) => context.Locate(t, baseObject));
    }

    public async Task<object?> GetLinkedAsync(object? baseObject,Type viewMode, Type viewClass)
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
        var linkedType = await _mvvm.GetLinkedTypeAsync(baseObject.GetType(), viewMode, viewClass);

        if (linkedType == null)
        {
            return null;
        }

        // we don't want to cache views cause they cannot be used twice
        if (typeof(IView).IsAssignableFrom(linkedType))
        {
            //linkedObject is View
            return context.Locate(linkedType, baseObject);
        }

        // baseObject is ViewModel
        var cache = _linked.GetOrCreateValue(baseObject);
        return cache.GetOrAdd(linkedType, (t) => context.Locate(t, baseObject));
    }
    public async Task<IView?> GetViewAsync(object? baseObject, Type? viewMode, Type? viewClass, CancellationToken token = default)
    {
        //TODO : find a solution to identify baseObject type when it's null and retrieve an empty view
        if (baseObject == null) return null;

        viewClass ??= typeof(IDefaultViewClass);
        viewMode ??= typeof(DefaultViewMode);

        while (true)
        {
            try
            {
                var linked = await GetLinkedAsync(baseObject, viewMode, viewClass, token);
                if (token.IsCancellationRequested) return null;

                switch (linked)
                {
                    case null:
                        return await _mvvm.GetNotFoundViewAsync(baseObject.GetType(), viewMode, viewClass, token);

                    case IView fe:
                        await _mvvm.PrepareViewAsync(fe, token);
                        return fe;

                    default:
                        baseObject = linked;
                        break;
                }

            }
            catch (Exception e)
            {
                if (e is OperationCanceledException)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }
    }

    // TODO 
    //public event DependencyPropertyChangedEventHandler ViewDataContextChanged;
    //private void View_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    //{
    //    ViewDataContextChanged?.Invoke(sender, e);
    //}
}