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
using System.Linq;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Attributes;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm
{
    [Export(typeof(IMvvmContext))]
    public class MvvmContext : IMvvmContext//, IInitializer
    {
        private readonly ConcurrentDictionary<Type, ConcurrentQueue<Action<IMvvmContext,object>>> _creators = new();

        private readonly Lazy<ViewModelCache> _cache;

        public IExportLocatorScope Scope { get; }
        public IMvvmService Mvvm { get; }

        public string Name { get; }
        public IMvvmContext Parent { get; }

        public MvvmContext(IMvvmService mvvm, object parent, string name, [Import]IExportLocatorScope scope)
        {
            Scope = scope;
            Mvvm = mvvm;

            if(parent is IMvvmContext ctx)
                Parent = ctx;
            Name = name;
            _cache = new Lazy<ViewModelCache>(() => new ViewModelCache(this,Mvvm));
        }

        public IMvvmContext GetChildContext(string name)
        {
            var ctx = Mvvm.GetNewContext(this,name);
            return ctx;
        }

        public IMvvmContext AddCreator<T>(Action<T> action)
        {
            var list = _creators.GetOrAdd(typeof(T), t => new ConcurrentQueue<Action<IMvvmContext,object>>());
            list.Enqueue((ctx,e) => action((T) e));
            return this;
        }
        public IMvvmContext AddCreator<T>(Action<IMvvmContext,T> action)
        {
            var list = _creators.GetOrAdd(typeof(T), t => new ConcurrentQueue<Action<IMvvmContext,object>>());
            list.Enqueue((ctx,e) => action(ctx,(T) e));
            return this;
        }

        /// <summary>
        /// Initialise newly created linked using creators
        /// </summary>
        /// <param name="linked"></param>
        public void CallCreators(object linked)
        {
            foreach (var kv in _creators.Where(t => t.Key.IsInstanceOfType(linked)))
            {
                foreach (var creator in kv.Value)
                {
                    creator(this,linked);
                }
            }
        }

        public object GetLinked(object o, Type viewMode, Type viewClass) => _cache.Value.GetLinked(o, viewMode,viewClass);


        public IView GetView(object baseObject) => GetView(baseObject,typeof(ViewModeDefault),typeof(IViewClassDefault));

        public IView GetView(object baseObject, Type viewMode, Type viewClass)
            => _cache.Value.GetView(baseObject,viewMode,viewClass);

        //private readonly Func<Type, object> _locate ;
        public T Locate<T>( object baseObject = null) => (T)Locate(typeof(T), baseObject);


        public object Locate(Type type, object baseObject = null)
        {
            // TODO check
            //return Locate(() => Scope.Locate(RuntimeImportContext.GetStatic(this, type)), baseObject);
            return Locate(() => Scope.Locate(type),baseObject);
        }

        public T Locate<T>(Func<T> locate, object baseObject = null)
        {
            return (T) Locate(new Func<object>(() => locate()), baseObject);
        }

        public object Locate(Func<object> locate, object baseObject = null)
        {
            var obj = locate();
            switch (obj)
            {
                case IView v:
                    var h = Mvvm.ViewHelperFactory.Get(v, hh => hh.Context=this);
                    h.Linked = baseObject;
                    CallCreators(v);
                    break;
                case IViewModel vm:
                        IMvvmContext context = this;
                        if (vm is IMvvmContextProvider p)
                        {
                            context = GetChildContext(vm.GetType().Name);
                            p.ConfigureMvvmContext(context);
                        }
                        vm.MvvmContext = context;
                        vm.Model = baseObject;

                        CallCreators(vm);
                    break;
            }
            if (baseObject is IViewModel vmb)
            {
                vmb.SetLinked(obj);
            }
            return obj;
        }

        // TODO : missing
        //public void Initialize(IRuntimeImportContext ctx, object[] args)
        //{
        //    Mvvm = ctx.GetTarget<IMvvmService>();
        //}
    }
}
