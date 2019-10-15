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
            var injectBefore = Scope.GetClassInjector(tree,InjectLocation.BeforeConstructor);
            var injectAfter = Scope.GetClassInjector(tree,InjectLocation.AfterConstructor);

            var ctor = tree.Key.GetConstructor();
            var type = tree.Key.ReturnType;


            if (ctor == null)
            {
                return (c, args) =>
                {
                    var o = FormatterServices.GetUninitializedObject(type);
                    c = c.Get(o);
                    injectBefore(c, null, o);
                    injectAfter(c, null, o);

                    return o;
                };

                string p = "";
                if (tree.Key.Signature != null)
                    p = tree.Key.Signature.Types.Select(e => e.Name).Aggregate((cc, n) => $"{cc},{n}");

                var message = "Locate " + tree.Key.ReturnType.GenericReadableName() + " no compatible constructor(" + p + ") found";
                throw new Exception(message);
            }

            return (c, args) =>
            {
                //var o = FormatterServices.GetSafeUninitializedObject(type);
                var o = FormatterServices.GetUninitializedObject(type);
                c = c.Get(o);
                injectBefore(c, null, o);
                ctor.Invoke(o, args);
                injectAfter(c, null, o);

                return o;
            };
        }

        public void SetSingletonLocator()
        {
            var locator = Locator;

            Locator = (tree) =>
            {
                var type = ExportType(tree);

                tree.Key = new ActivatorKey(type, null);

                var singleton = (Scope as DependencyInjectionContainer).Singletons.GetOrAdd(type,
                    t => locator(tree)(
                        RuntimeImportContext.GetStatic(null, tree.Context.ImportType), null));

                return (ric, a) => singleton;

            };
        }
    }
}
