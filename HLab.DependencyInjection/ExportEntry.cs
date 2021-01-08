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
        public ClassActivator(DependencyLocator activator, DependencyInjector injector)
        {
            Activator = activator;
            Injector = injector;
        }

        public DependencyLocator Activator { get; }
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
        public Func<IActivatorTree, DependencyLocator> Locator { get; set; }
        private Func<IActivatorTree, bool> _condition;

        public Func<IRuntimeImportContext, bool> RuntimeCondition { get; set; }
        public ExportMode Mode { get; private set; }
        public int Priority { get; private set; } = 0;

        public void AddMode(ExportMode mode) => this.Mode |= mode;
        public void SetPriority(int priority) => Priority = priority;

        private readonly ConcurrentDictionary<IActivatorKey, DependencyLocator> _activators
            = new ConcurrentDictionary<IActivatorKey, DependencyLocator>();

        public ExportEntry(IExportLocatorScope container)
        {
            Scope = container;
            Locator = GetActivator;
        }

        public IActivatorKey GetActivatorKey(IActivatorTree tree) => new ActivatorKey(ExportType(tree), tree.Context.Signature);

        private DependencyLocator GetActivator(IActivatorTree tree)
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

        private DependencyLocator GetNewClassActivator(IActivatorTree tree)
        {

            var ctor = tree.Key.GetConstructor();
            var type = tree.Key.ReturnType;

            var inject = Scope.GetClassInjector(tree);

            DependencyInjector locator = inject.Item1;

            if(ctor!=null) 
            {
                if(ctor.GetParameters().Length>0)
                    locator += (ctx,args,o) => ctor.Invoke(o,args);
                else if(inject.Item2==null)
                    locator += (ctx,args,o) => ctor.Invoke(o,null);
                else
                        locator += inject.Item2;
            }
            else locator += inject.Item2;

            locator += inject.Item3;

            return (c, args) =>
            {
                var o = FormatterServices.GetUninitializedObject(type);
                c = c.Get(o);
                locator?.Invoke(c, args, o);
                return o;
            };
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
                        return locator(tree)(
                            RuntimeImportContext.GetStatic(null, tree.Context.ImportType), null);
                    });

                return (ric, a) => singleton;

            };
        }
    }
}
