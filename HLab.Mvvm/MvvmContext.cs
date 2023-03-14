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
using System.Linq;
using System.Threading.Tasks;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm
{
    public class MvvmContext : IMvvmContext//, IInitializer
    {
        readonly ConcurrentDictionary<Type, ConcurrentQueue<Action<IMvvmContext, object>>> _creators = new();

        readonly ViewModelCache _cache;

        public IMvvmService Mvvm => _mvvm;
        readonly MvvmService _mvvm;

        public string Name { get; }
        public IMvvmContext Parent { get; }

        public MvvmContext(IMvvmContext parent, string name, MvvmService mvvm)
        {
            Name = name;

            _mvvm = mvvm;
            Parent = parent;

            _cache = new ViewModelCache(this, mvvm);
        }

        public IMvvmContext GetChildContext(string name) => new MvvmContext(this,name,_mvvm);

        public IMvvmContext AddCreator<T>(Action<T> action)
        {
            var list = _creators.GetOrAdd(typeof(T), t => new ConcurrentQueue<Action<IMvvmContext, object>>());
            list.Enqueue((ctx, e) => action((T)e));
            return this;
        }
        public IMvvmContext AddCreator<T>(Action<IMvvmContext, T> action)
        {
            var list = _creators.GetOrAdd(typeof(T), t => new ConcurrentQueue<Action<IMvvmContext, object>>());
            list.Enqueue((ctx, e) => action(ctx, (T)e));
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
                    creator(this, linked);
                }
            }
        }

        public object GetLinked(object o, Type viewMode, Type viewClass) 
            => _cache.GetLinked(o, viewMode, viewClass);

        public IView? GetView(object baseObject) 
            => GetView(baseObject, typeof(DefaultViewMode), typeof(IDefaultViewClass));

        public IView? GetView(object baseObject, Type viewMode, Type viewClass)
            => _cache.GetView(baseObject, viewMode, viewClass);

        public Task<IView?> GetViewAsync(object baseObject, Type viewMode, Type viewClass)
            => _cache.GetViewAsync(baseObject, viewMode, viewClass);

        //private readonly Func<Type, object> _locate ;
        public T Locate<T>(object baseObject = null) 
            => (T)Locate(typeof(T), baseObject);


        public object Locate(Type type, object baseObject = null) 
            =>
            Locate(() => _mvvm.LocateFunc?.Invoke(type), baseObject); // _locateFunc();

        public T Locate<T>(Func<T> locate, object baseObject = null) 
            => (T)Locate(new Func<object>(() => locate()), baseObject);

        public object Locate(Func<object> locate, object baseObject = null)
        {
            var obj = locate();
            switch (obj)
            {
                case IView v:
                    var h = Mvvm.ViewHelperFactory.Get(v, hh => hh.Context = this);
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
