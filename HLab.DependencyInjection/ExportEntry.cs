using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using HLab.DependencyInjection.Annotations;

namespace HLab.DependencyInjection
{
    public class ClassActivator
    {
        public ClassActivator(IDependencyLocator activator, DependencyInjector injector)
        {
            Activator = activator;
            Injector = injector;
        }

        public IDependencyLocator Activator { get; }
        public DependencyInjector Injector { get; }
    }

    public class ExportEntry : IExportEntry
    {
        public IExportLocatorScope Scope { get; private set; }

        public override string ToString() => DefaultExportType.Name;

        public Type DefaultExportType
        {
            get
            {
                var tree = new ActivatorTree(null,new ImportContext());
                return ExportType(tree);
            }
        }

        public Func<IActivatorTree,Type> ExportType { get; set; }
        public Func<IActivatorTree, IDependencyLocator> Locator { get; set; }
        private Func<IActivatorTree, bool> _condition;

        public ExportMode Mode { get; private set; }
        public int Priority { get; private set; } = 0;

        public void AddMode(ExportMode mode) => this.Mode |= mode;
        public void SetPriority(int priority) => Priority = priority;

        private readonly ConcurrentDictionary<IActivatorKey, IDependencyLocator> _activators
            = new();

        public ExportEntry(IExportLocatorScope container)
        {
            Scope = container;
            Locator = GetActivator;
        }

        public IActivatorKey GetActivatorKey(IActivatorTree tree) => new ActivatorKey(ExportType(tree), tree.Context.Signature);

        private IDependencyLocator GetActivator(IActivatorTree tree)
        {
            var key = new ActivatorKey(ExportType(tree), tree.Context.Signature);
            tree.Key = key;
#if DEBUG
            Debug.WriteLine(tree.TabbedBranch());
#endif

            return _activators.GetOrAdd(key, k => GetNewClassActivator(tree));
        }

        public bool Test(IActivatorTree tree) => _condition?.Invoke(tree) ?? true;

        public void AndCondition(Func<IActivatorTree, bool> condition)
        {
            var oldCondition = _condition;
            var cnd = (oldCondition == null)
                ? condition
                : ctx => oldCondition(ctx) && condition(ctx);
            _condition = cnd;
        }
        public void OrCondition(Func<IActivatorTree, bool> condition)
        {
            var oldCondition = _condition;
            var cnd = (oldCondition == null)
                ? condition
                : ctx => oldCondition(ctx) || condition(ctx);
            _condition = cnd;
        }

        private IDependencyLocator GetNewClassActivator(IActivatorTree tree)
        {

            var ctor = tree.Key.GetConstructor();
            var type = tree.Key.ReturnType;

            var inject = Scope.GetClassInjector(tree);

            var locator = default(DependencyInjector);

            if(ctor!=null) 
            {
                if(ctor.GetParameters().Length>0)
                    locator += (args,o) => ctor.Invoke(o,args);
                else if(inject.Constructor==null)
                    locator += (args,o) => ctor.Invoke(o,null);
                else
                        locator += inject.Constructor;
            }
            else locator += inject.Constructor;

            locator += inject.PostConstructor;

            return new DependencyLocator<object>((args) =>
            {
                var o = FormatterServices.GetUninitializedObject(type);
                locator?.Invoke(args, o);
                return o;
            });
        }

        public void SetSingletonLocator()
        {
            var locator = Locator;

            Locator = (tree) =>
            {
                var type = ExportType(tree); 


                var singleton = (Scope as DependencyInjectionContainer).Singletons.GetOrAdd(type,
                    t =>
                    {
                        tree.Key = new ActivatorKey(type, null);
                        return locator(tree).Locate(
                            RuntimeImportContext.GetStatic(null, tree.Context.ImportType), null);
                    });

                return new DependencyLocator<object>(a => singleton);

            };
        }
    }
}
