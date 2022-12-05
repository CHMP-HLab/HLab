using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using HLab.Base;
using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm
{
    public abstract class MvvmService : IMvvmService
    {
        IMessagesService _messageBus;

        string _assemblyName;

        public IMvvmContext GetNewContext(IMvvmContext parent, string name) => new MvvmContext(parent, name, this); 

        public MvvmService(IMessagesService messageBus, Func<Type,object> locateFunc)
        {
            LocateFunc = locateFunc;
            _messageBus = messageBus;
            ServiceState = ServiceState.NotConfigured;
            MainContext = new MvvmContext(null,"root",this);
            _assemblyName = Assembly.GetAssembly(typeof(IView)).GetName().Name;
        }

        public Func<Type,object> LocateFunc {get; private set;}

        public abstract void PrepareView(object view);

        public IMvvmContext MainContext { get; private set;}

        public HelperFactory<IViewHelper> ViewHelperFactory { get; } = new HelperFactory<IViewHelper>();

        public Type GetLinkedType(Type getType, Type viewMode, Type viewClass)
        {
            if (getType.IsConstructedGenericType)
            {
                getType = getType.GetGenericTypeDefinition();
            }

            if(_entries.TryGetValue(getType, out var best))
            {
                var result = best.GetLinked(viewClass, viewMode);
                if (result.LinkedType != null) return result.LinkedType;
            }

            var level = int.MaxValue;
            Type linkedType = null;
            var select = _entries.Where(e => e.Key.IsAssignableFrom(getType));
            foreach(var entry in select)
            {
                var t = getType;
                var l = 0;
                while(t.BaseType!=null && entry.Key.IsAssignableFrom(t)) { t = t.BaseType; l++; }

                if (l < level)
                {
                    var lt = entry.Value.GetLinked(viewClass, viewMode).LinkedType;
                    if(lt!=null)
                    {
                        linkedType = lt;
                        level = l;
                    }
                }
            }

            if (linkedType == null)
            {
                var baseMode = viewMode.BaseType;
                if (baseMode == typeof(ViewMode)) return null;
                linkedType = GetLinkedType(getType, baseMode, viewClass);
                Register(getType, linkedType, viewClass, viewMode);
            }
            return linkedType;
        }

        public abstract IView GetNotFoundView(Type getType, Type viewMode, Type viewClass);
        protected abstract void Register(Type t);

        /// <summary>
        /// Register all assemblies referencing this (HLab.Mvvm).
        /// </summary>
        public virtual void Register()
        {
            var assemblies = AssemblyHelper.GetReferencingAssemblies(_assemblyName).ToList();
            _perAssemblyProgress = 1.0 / assemblies.Count;
            foreach (var assembly in assemblies)
            {
                Register(assembly);
            }

            ServiceState = ServiceState.Available;
        }

        double _perAssemblyProgress = 0.0;
        double _progress = 0.0;

        public void Register(Assembly assembly)
        {
            // Find all views and register it
            var views = assembly.GetTypesSafe().Where(t => typeof(IView).IsAssignableFrom(t)).ToList();


            if (views.Count == 0)
            {
                _progress += _perAssemblyProgress;
                OnProgress(_progress, assembly.GetName().Name);
            }
            else
            {
                var perViewProgress = _perAssemblyProgress / views.Count;

                foreach (var viewType in views)
                {
                    OnProgress(_progress, viewType.Name);
                    foreach (var t in viewType.GetInterfaces().Where(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IView<,>)
                        ))
                    {
                        var viewMode = t.GetGenericArguments()[0];
                        var baseType = t.GetGenericArguments()[1];
                        var viewClasses = GetViewClasses(viewType).ToList();

                        OnProgress(_progress,
                            viewType.Name + " - " + viewMode.Name + " - " + baseType.Name + " - " + viewClasses.Count);

                        if (viewClasses.Count > 0)
                        {
                            foreach (var cls in viewClasses)
                                RegisterAll(baseType, viewType, cls, viewMode);
                        }
                        else
                        {
                            RegisterAll(baseType, viewType, typeof(IViewClassDefault), viewMode);
                        }
                    }

                    _progress += perViewProgress;
                }
            }
        }

        public void RegisterAll(
            Type baseType
            , Type linkedType
            , Type viewClass
            , Type viewMode
            /*, Type regFrom = null*/)
        {
            Register(baseType, linkedType /*lt*/, viewClass, viewMode);


            var basesTypes = AllAssemblies().SelectMany(a => a.GetTypesSafe().Where(baseType.IsAssignableFrom).Where(t => !t.IsAssignableFrom(baseType)).Where(t => !typeof(IViewModelDesign).IsAssignableFrom(t)));
            var linkedTypes = AllAssemblies().SelectMany(a => a.GetTypesSafe().Where(linkedType.IsAssignableFrom)).ToList();

            foreach (var bt in basesTypes)
                foreach (var lt in linkedTypes)
                    Register(bt, lt /*lt*/, viewClass, viewMode);

        }


        public void Register(
            Type baseType
            , Type linkedType
            , Type viewClass
            , Type viewMode
            )
        {
            if (!viewClass.IsInterface) throw new ArgumentOutOfRangeException("viewClass must be interface " + viewClass.Name);

            if (typeof(IViewModelDesign).IsAssignableFrom(linkedType)) return;

            Debug.WriteLine(baseType.Name+"->"+linkedType.Name+":"+viewClass.Name+"#"+viewMode.Name );
            Debug.Assert(baseType!=linkedType);

            var entry = _entries.GetOrAdd(
                baseType,
                (t) => {
                    Register(t);
                    return new MvvmBaseEntry(t); }
                );

            entry.Register(linkedType, viewClass, viewMode);

            var modelType = GetModelType(baseType);
            if (modelType != null)
                Register(modelType, baseType, viewClass, viewMode);

        }



        /// <summary>
        /// Get all ViewClass assigned to a specific type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IEnumerable<Type> GetViewClasses(Type type)
        {
            return type.GetInterfaces().Where(i => typeof(IViewClass).IsAssignableFrom(i) && typeof(IViewClass) != i);
        }


        readonly ConcurrentDictionary<Type, Type> _modelsTypes = new ConcurrentDictionary<Type, Type>();

        Type GetModelType(Type type)
        {
            return _modelsTypes.GetOrAdd(type, (t) => {
                //if (!typeof(IViewModel).IsAssignableFrom(type)) return null; //throw new ArgumentException(type + " does not implement IViewModel");
                foreach (var iface in t.GetInterfaces())
                {
                    var curiface = iface.IsGenericType ? iface.GetGenericTypeDefinition() : iface;
                    if (curiface != typeof(IViewModel<>)) continue;

                    if (iface.GenericTypeArguments.Length > 0)
                    {
                        var modelType = iface.GenericTypeArguments[0];
                        Debug.Assert(modelType!=type);
                        return modelType;
                    }
                }
                return null;
            });
        }

        IEnumerable<Assembly> AllAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies()/*.Where(a => a.GetReferencedAssemblies().Any(e => e.Name == "Mvvm"))*/;
        }

        readonly ConcurrentDictionary<Type, MvvmBaseEntry> _entries = new ConcurrentDictionary<Type, MvvmBaseEntry>();

        protected virtual void OnProgress(double progress, string text)
        {
            _messageBus.Publish(new ProgressMessage(progress,text));
        }

        internal class MvvmBaseEntry
        {
            public Type BaseType { get; }
            readonly HashSet<MvvmLinkedEntry> _list = new HashSet<MvvmLinkedEntry>();
            public MvvmBaseEntry(Type baseType)
            {
                BaseType = baseType;
            }

            public void Register(Type linkedType, Type viewClass, Type viewMode)
            {
                _list.Add(new MvvmLinkedEntry(linkedType, viewClass, viewMode));
            }

            public MvvmLinkedEntry GetLinked(Type viewClass, Type viewMode)
            {

                if (viewClass == null) viewClass = typeof(IViewClassDefault);
                if (viewMode == null) viewMode = typeof(ViewModeDefault);

                var result = new MvvmLinkedEntry();
                foreach(var e in _list)
                {
                    if (!e.ViewClass.IsAssignableFrom(viewClass)) continue;
                    if (!e.ViewMode.IsAssignableFrom(viewMode)) continue;

                    if (result.LinkedType == null) { result = e; continue; } // initial

                    if(result.ViewClass.IsAssignableFrom(e.ViewClass)) { result = e; continue; } // better viewClass
                    if(result.ViewMode.IsAssignableFrom(e.ViewMode)) { } // better viewMode

                    if(result.ViewMode.IsAssignableFrom(e.ViewMode)) { } // better linked type
                }

                return result;
            }
        }

        internal readonly struct MvvmLinkedEntry
        {
            public readonly Type LinkedType;
            public readonly Type ViewClass;
            public readonly Type ViewMode;
            public readonly bool Cacheable;

            public override string ToString() => LinkedType.Name + "(" + ViewClass.Name + ":" + ViewMode.Name + ")";

            public MvvmLinkedEntry(Type linkedType, Type viewClass, Type viewMode)
            {
                LinkedType = linkedType;
                ViewClass = viewClass;
                ViewMode = viewMode;
                var attr = LinkedType.GetCustomAttribute<MvvmCacheabilityAttribute>();
                Cacheable = (attr==null || attr.Cacheability == MvvmCacheability.Cacheable) ;
            }

            public override bool Equals(object other)
            {
                if (other is MvvmLinkedEntry e)
                {
                    return EqualityComparer<Type>.Default.Equals(LinkedType, e.LinkedType) &&
                           EqualityComparer<Type>.Default.Equals(ViewClass, e.ViewClass) &&
                           EqualityComparer<Type>.Default.Equals(ViewMode, e.ViewMode);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return System.HashCode.Combine(LinkedType, ViewClass, ViewMode);
            }

            public static bool operator ==(MvvmLinkedEntry c1, MvvmLinkedEntry c2)
            {
                return c1.Equals(c2);
            }

            public static bool operator !=(MvvmLinkedEntry c1, MvvmLinkedEntry c2)
            {
                return !c1.Equals(c2);
            }
        }

        public ServiceState ServiceState { get; private set; }
    }
}
