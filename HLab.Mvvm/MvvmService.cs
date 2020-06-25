using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


using HLab.Base;
using HLab.Core;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm
{
    public abstract class MvvmService : Service, IMvvmService
    {
        [Import]
        private readonly IMessageBus _messageBus;

        private readonly string _assemblyName;
        /// <summary>
        /// Dict_baseType => dict viewMode => 
        /// </summary>
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, ConcurrentDictionary<Type, Tuple<Type, Type>>>> _links = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, ConcurrentDictionary<Type, Tuple<Type, Type>>>>();


        [Import] private readonly Func<IMvvmContext, string, IMvvmContext> _getNewContext;

        public IMvvmContext GetNewContext(IMvvmContext parent, string name) => _getNewContext(parent, name);


        protected MvvmService()
        {
            ServiceState = ServiceState.NotConfigured;
            MainContext = _getNewContext(null,"root");
            _assemblyName = Assembly.GetAssembly(typeof(IView)).GetName().Name;
        }

        public abstract void PrepareView(object view);

        public IMvvmContext MainContext { get; }

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

            if (linkedType == null && viewMode != typeof(ViewModeDefault))
                linkedType = GetLinkedType(getType, typeof(ViewModeDefault), viewClass);


            Register(getType, linkedType, viewClass, viewMode);
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

        private double _perAssemblyProgress = 0.0;
        private double _progress = 0.0;

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
            var basesTypes = AllAssemblies().SelectMany(a => a.GetTypesSafe().Where(baseType.IsAssignableFrom).Where(t => !typeof(IViewModelDesign).IsAssignableFrom(t)));
            //var linkedTypes = AllAssemblies().SelectMany(a => a.GetTypesSafe().Where(linkedType.IsAssignableFrom));

            foreach (var bt in basesTypes)
               // foreach (var lt in linkedTypes)
                    Register(bt, linkedType /*lt*/, viewClass, viewMode);

        }
        public void Register(
            Type baseType
            , Type linkedType
            , Type viewClass
            , Type viewMode
            )
        {
            if (typeof(IViewModelDesign).IsAssignableFrom(linkedType)) return;

            var e = _entries.GetOrAdd(
                baseType,
                (t) => {
                    Register(t);
                    return new MvvmBaseEntry(t); }
                );

            e.Register(linkedType, viewClass, viewMode);

            var modelType = GetModelType(baseType);
            if (modelType != null)
                RegisterAll(modelType, baseType, viewClass, viewMode);

        }
        /// <summary>
        /// Get all ViewClass assigned to a specific type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private IEnumerable<Type> GetViewClasses(Type type)
        {
            return type.GetInterfaces().Where(i => typeof(IViewClass).IsAssignableFrom(i) && typeof(IViewClass) != i);
        }


        private readonly ConcurrentDictionary<Type, Type> _modelsTypes = new ConcurrentDictionary<Type, Type>();
        private Type GetModelType(Type type)
        {
            return _modelsTypes.GetOrAdd(type, (t) => {
                //if (!typeof(IViewModel).IsAssignableFrom(type)) return null; //throw new ArgumentException(type + " does not implement IViewModel");
                foreach (var iface in t.GetInterfaces())
                {
                    var curiface = iface.IsGenericType ? iface.GetGenericTypeDefinition() : iface;
                    if (curiface != typeof(IViewModel<>)) continue;

                    if (iface.GenericTypeArguments.Length > 0)
                    {
                        return iface.GenericTypeArguments[0];
                    }
                }
                return null;
            });
        }
        private IEnumerable<Assembly> AllAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies()/*.Where(a => a.GetReferencedAssemblies().Any(e => e.Name == "Mvvm"))*/;
        }

        private ConcurrentDictionary<Type, MvvmBaseEntry> _entries = new ConcurrentDictionary<Type, MvvmBaseEntry>();

        protected virtual void OnProgress(double progress, string text)
        {
            _messageBus.Publish(new ProgressMessage(progress,text));
        }

        private class MvvmBaseEntry
        {
            public readonly Type BaseType;
            private readonly HashSet<MvvmLinkedEntry> _list = new HashSet<MvvmLinkedEntry>();
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

                MvvmLinkedEntry result = new MvvmLinkedEntry();
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

        private struct MvvmLinkedEntry
        {
            public readonly Type LinkedType;
            public readonly Type ViewClass;
            public readonly Type ViewMode;

            public override string ToString() => LinkedType.Name + "(" + ViewClass.Name + ":" + ViewMode.Name + ")";

            public MvvmLinkedEntry(Type linkedType, Type viewClass, Type viewMode)
            {
                LinkedType = linkedType;
                ViewClass = viewClass;
                ViewMode = viewMode;
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
                var hashCode = -946743261;
                hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(LinkedType);
                hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(ViewClass);
                hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(ViewMode);
                return hashCode;
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

    }
}
