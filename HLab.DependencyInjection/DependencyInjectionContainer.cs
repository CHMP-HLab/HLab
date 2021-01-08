using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using HLab.Base;
using HLab.DependencyInjection.Activators;
using HLab.DependencyInjection.Annotations;

namespace HLab.DependencyInjection
{

    public class DependencyInjectionContainer : IExportLocatorScope
    {
        public IExportLocatorScope Configure(Func<IConfigurator,IConfigurator> configurator)
        {
            
            var c = configurator(new Configurator(this)) as Configurator;
            foreach (var entry in c.Entries)
            {
                if(entry.Mode.HasFlag(ExportMode.Decorator))
                    _decoratorEntries.Add(entry);
                else
                    _locatorEntries.Add(entry);
            }
 
            return this;
        }

        public IExportLocatorScope AutoConfigure<T>(Func<IConfigurator, IConfigurator> configurator)
        {
             _autoConfigure.TryAdd(typeof(T), configurator);
            return this;
        }

        private readonly ConcurrentDictionary<Type, Func<IConfigurator, IConfigurator>> _autoConfigure = new ConcurrentDictionary<Type, Func<IConfigurator, IConfigurator>>();


        private readonly ConcurrentDictionary<IImportContext, DependencyLocator> _locators = new ConcurrentDictionary<IImportContext, DependencyLocator>();

        private readonly WeightedList<IExportEntry> _locatorEntries = new WeightedList<IExportEntry>().AddComparator((a,b) => a.Value.Priority - b.Value.Priority);
        private readonly List<IExportEntry> _decoratorEntries = new List<IExportEntry>();


        private readonly ConcurrentDictionary<Type, Tuple<DependencyInjector,DependencyInjector,DependencyInjector>> _injectors
            = new ConcurrentDictionary<Type, Tuple<DependencyInjector,DependencyInjector,DependencyInjector>>();

        internal readonly ConcurrentDictionary<Type, object> Singletons = new ConcurrentDictionary<Type, object>();
        //private readonly ConcurrentDictionary<Type,List<IMappedCondition>> _mapped = new ConcurrentDictionary<Type, List<IMappedCondition>>();
        private readonly ConcurrentDictionary<Type, List<DependencyInjector>> _initializers = new ConcurrentDictionary<Type, List<DependencyInjector>>();

        public void ExportInitialize<T>(Action<IRuntimeImportContext, object[], T> action)
        {
            Initializer(typeof(T),(c,a,t)=>action(c,a,(T)t));

        }
        public void Initializer(Type type, DependencyInjector action)
        {
            var list = _initializers.GetOrAdd(type, t => new List<DependencyInjector>());
            list.Add(action);
        }

        public void StaticInjection() => _staticInjection?.Invoke();
        private Action _staticInjection = null;
             
        public void ExportAssembly(Assembly assembly)
        {
            var types = assembly.GetTypesSafe().Where(t => t.IsClass && (!t.IsAbstract || t.IsSealed));
            foreach (var type in types)
            {

                foreach (var k in _autoConfigure)
                {
                    if (k.Key.IsAssignableFrom(type))
                        Configure(c => k.Value(c.Export(type)));
                }

                try { 
                    if(type.GetCustomAttributes<ExportAttribute>().Any())
                        Configure( e => e.Export(type).AsAnnotated());
                }
                catch(FileNotFoundException)
                { }

                var flag = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

                var properties = type.GetProperties(flag).Where(p => p.GetCustomAttributes<ImportAttribute>().Any());
                if(type.Name.Contains("WorkflowAnalysisExtension"))
                { }
                foreach (var p in properties)
                {
                    _staticInjection += () => p.SetValue(null,Locate(p.PropertyType)) ;
                }
                var fields = type.GetFields(flag).Where(p => p.GetCustomAttributes<ImportAttribute>().Any());
                foreach (var f in fields)
                {
                    _staticInjection += () => f.SetValue(null,Locate(f.FieldType));
                }
                var methods = type.GetMethods(flag).Where(p => p.GetCustomAttributes<ImportAttribute>().Any());
                foreach (var m in methods)
                {
                    var parameters = m.GetParameters();


                    _staticInjection += () =>
                    {
                        var param = new object[parameters.Length];
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            param[i] = Locate(parameters[i].ParameterType);
                        }

                        m.Invoke(null, param);
                    } ;
                }
            }

        }


        public object Locate(Type type, object target = null) 
            => Locate(RuntimeImportContext.GetStatic(target, type));


        public T Locate<T>(object target = null) 
            => (T)Locate(RuntimeImportContext.GetStatic(target, typeof(T)));

        public T Locate<T>(object target, IImportContext ctx)
            => (T)Locate(RuntimeImportContext.GetStatic(target, ctx));

        public object Locate(IRuntimeImportContext ctx, object[]args = null) 
            => GetLocator(new ActivatorTree(null,ctx.StaticContext))(ctx,args);


        private DependencyLocator GetLocator(IActivatorTree tree) 
            => _locators.GetOrAdd(tree.Context, c => GetNewLocator(tree));


        public void Inject(object obj, object[] args, IRuntimeImportContext ctx)
        {
            var tree = new ActivatorTree(null, ctx.StaticContext);
            tree.Key = new ActivatorKey(obj.GetType(),null);
            var injectors = GetClassInjector(tree);
            injectors.Item1(ctx, args, obj);
            injectors.Item2(ctx, args, obj);
        }


        private DependencyLocator GetNewLocator_IEnumerable_1<T>(IActivatorTree tree)
        {
            Action<IRuntimeImportContext, object[], List<T>> action = (c,a,l) => {};

            var testTree = new ActivatorTree(tree, tree.Context.Get(typeof(T)));

            foreach (var e in _locatorEntries.Where(l => l.Test(testTree)).ToList())
            {
                var activator = e.Locator(new ActivatorTree(tree, tree.Context.Get(typeof(T))));

                action += (c,a,l) => l.Add((T)activator(c,a));
            }
            return (c,a) =>
            {
                var l = new List<T>();
                action(c,a,l);
                return l;
            };
        }

        private DependencyLocator GetNewLocator_Lazy_1<T>(ActivatorTree tree)
        {
            tree = new ActivatorTree(tree,tree.Context.Get(typeof(T)));

            var locator = GetLocator(tree);
            return (c, a) => new Lazy<T>(() => (T)locator(c, a));
        }

        private DependencyLocator GetNewLocator_Func_1<T>(ActivatorTree tree)
        {
            tree = new ActivatorTree(tree, tree.Context.Get(typeof(T)));
            var locator = GetLocator(tree);
            return (c, a) => new Func<T>(() => (T)locator(c, a));
        }

        private DependencyLocator GetNewLocator_Func_2<T, TResult>(ActivatorTree tree)
        {
            tree = new ActivatorTree(tree, tree.Context.Get(typeof(TResult), new MethodSignature(typeof(T))));
            var locator = GetLocator(tree);
            return (c, a) => new Func<T, TResult>((t) => (TResult)locator(c, new object[] { t }));
        }
        private DependencyLocator GetNewLocator_Func_3<T1, T2, TResult>(ActivatorTree tree)
        {
            tree = new ActivatorTree(tree, tree.Context.Get(typeof(TResult), new MethodSignature(typeof(T1), typeof(T2))));
            var locator = GetLocator(tree);
            return (c, a) => new Func<T1, T2, TResult>((t1, t2) => (TResult)locator(c, new object[] { t1, t2 }));
        }
        private DependencyLocator GetNewLocator_Func_4<T1, T2, T3, TResult>(ActivatorTree tree)
        {
            tree = new ActivatorTree(tree, tree.Context.Get(typeof(TResult), new MethodSignature(typeof(T1), typeof(T2), typeof(T3))));
            var locator = GetLocator(tree);
            return (c, a) => new Func<T1, T2, T3, TResult>((t1, t2, t3) => (TResult)locator(c, new object[] { t1, t2, t3 }));
        }
        private DependencyLocator GetNewLocator_Func_5<T1, T2, T3, T4, TResult>(ActivatorTree tree)
        {
            tree = new ActivatorTree(tree, tree.Context.Get(typeof(TResult), new MethodSignature(typeof(T1), typeof(T2), typeof(T3), typeof(T4))));
            var locator = GetLocator(tree);
            return (c, a) => new Func<T1, T2, T3, T4, TResult>((t1, t2, t3, t4) => (TResult)locator(c, new object[] { t1, t2, t3, t4 }));
        }

        private DependencyLocator GetNewLocator(IActivatorTree tree)
        {
            var importType = tree.Context.ImportType;

            if (importType.IsGenericType)
            {
                var g = importType.GetGenericTypeDefinition();
                var args = importType.GetGenericArguments();

                // return a Func<Type,object> to provide generic locator without arguments
                if (args.Length==2 && args[0] == typeof(Type) && args[1] == typeof(object))
                    return (c, a) => new Func<Type, object>(t => Locate(c.Get(c.Target, tree.Context.Get(t,null))));

                //return a Func<Type,object[],object> to provide a generic locator with arguments
                if (args.Length == 3 && args[0] == typeof(Type) && args[1] == typeof(object[]) && args[2] == typeof(object))
                    return (c, a) => new Func<Type, object[], object>((t,ar) =>
                    {
                        var types = new MethodSignature(ar);
                        return Locate(c.Get(c.Target, tree.Context.Get(t,types)), ar);
                    });

                var name = g.Name.Split('`')[0];

                var mi = GetType().GetMethod("GetNewLocator_" + name + "_" + args.Length,BindingFlags.NonPublic|BindingFlags.Instance);
                if(mi != null)
                {
                    var m = mi.MakeGenericMethod(args);

                    var r = m?.Invoke(this, new object[] { tree });

                    if (r is DependencyLocator l) return l;

                    throw new Exception("");
                }

            }
            
            {
                var decorators = _decoratorEntries.Where(e => e.Test(tree));

                var exportEntry = _locatorEntries.Get(e => e.Test(tree));


                //if (tree.Context.ImportType.Name.Contains("ErpServices"))
                //{
                //    foreach (var e in _locatorEntries)
                //    {
                //        if (e.ExportType(tree).Name.Contains("ErpServices"))
                //        { }
                //        var r = e.Test(tree);
                //    }
                //}


                IActivatorKey export;
                ExportMode mode;

                // when no export entry found but type is instantiable use it TODO : should be an option
                if (exportEntry == null)
                {
                    if (!importType.IsAbstract && !importType.IsInterface)
                    {
                        exportEntry = new ExportEntry(this)
                        {
                            ExportType = t => importType
                        };

                        export = new ActivatorKey(importType,tree.Context.Signature);
                        mode = ExportMode.Default;
                    }
                    else
                    {
                        var t = new ActivatorTree(null,new ImportContext());
                        var n = _locatorEntries.Get(e =>
                        {
                            try
                            {
                                var exportType = e.ExportType(t);
                                return importType.IsAssignableFrom(exportType);
                            }
                            catch
                            {

                                return false;
                            }
                        });

                        throw new Exception("Unable to locate " + tree.ToString() /*importType.GenericReadableName()*/);
                    }
                }
                else
                {
                    export = exportEntry.GetActivatorKey(tree);
                    mode = exportEntry.Mode;
                }

                DependencyLocator activator;
                if (mode.HasFlag(ExportMode.Singleton))
                {
                    var type = export.ReturnType;

                    tree.Key = new ActivatorKey(type, null);

                    var singleton = Singletons.GetOrAdd(type, t => exportEntry.Locator(tree)(RuntimeImportContext.GetStatic(null,tree.Context.ImportType), null));

                    activator = (c, a) => singleton;
                }
                else
                    activator = exportEntry.Locator(tree);


                foreach (var de in decorators)
                {
                    var decorator = de.Locator(new ActivatorTree(tree,
                        tree.Context.Get(importType, new MethodSignature(importType))));

                    var old = activator;

                    activator = (c, a) => decorator(c, new[] { old(c, a) });
                }


                return activator;
            }
        }


        private readonly MethodInfo  _initializerMethodInfo = typeof(IInitializer).GetMethod("Initialize", new[] { typeof(RuntimeImportContext),typeof(object[]) });
        private Tuple<DependencyInjector,DependencyInjector,DependencyInjector> GetNewClassInjector(IActivatorTree tree)
        {
            var type = tree.Key.ReturnType;

            if (type.IsAbstract) throw new Exception("Unable to locate Abstract class : " + type.Name);
            if (type.IsInterface) throw new Exception("Unable to locate Interface : " + type.Name);

            DependencyInjector activator = null;
            DependencyInjector activatorCtor = null;
            DependencyInjector activatorAfter = null;


            var types = new Stack<Type>();
            var t = type;


            while (t != null)
            {
                types.Push(t);
                t = t.BaseType;
            }

            while (types.Count > 0)
            {
                t = types.Pop();
                foreach (var p in t.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    foreach (var attr in p.GetCustomAttributes<ImportAttribute>())
                    {
                        var ctx = ImportContext.Get(type, p).Get(typeof(IActivator));
                        var l = GetLocator(new ActivatorTree(tree, ctx));
                        var a = (IActivator)(l(RuntimeImportContext.GetStatic(null,ctx),null));

                        if(p is ConstructorInfo ci)
                            activatorCtor = a.GetActivator(GetLocator, new ActivatorTree(tree, ImportContext.Get(type, p)));
                        else
                        {
                            switch(attr.Location)
                            {
                                case InjectLocation.BeforeConstructor:
                                    activator += a.GetActivator(GetLocator, new ActivatorTree(tree, ImportContext.Get(type, p)));
                                    break;
                                case InjectLocation.AfterConstructor:
                                    activatorAfter += a.GetActivator(GetLocator, new ActivatorTree(tree, ImportContext.Get(type, p)));
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }


            if (typeof(IInitializer).IsAssignableFrom(type))
            {
                activator += (ctx, args, o) => _initializerMethodInfo.Invoke(o, new object []{ctx, args});
            }

            foreach (var k in _initializers)
            {
                if (k.Key.IsAssignableFrom(type))
                {
                    foreach (var action in k.Value)
                        activator += action;
                }
            }

            return new Tuple<DependencyInjector,DependencyInjector,DependencyInjector>(activator,activatorCtor,activatorAfter);
        }

        public Tuple<DependencyInjector,DependencyInjector,DependencyInjector> GetClassInjector(IActivatorTree tree)
            => _injectors.GetOrAdd(tree.Key.ReturnType, t => GetNewClassInjector(tree));

        public DependencyInjectionContainer()
        {
            Configure(c => c.Export<DependencyInjectionContainer>().Set(e => e.Locator = t => (ric,a) => this).As<IExportLocatorScope>());

            Configure(c => c
                .Export<FieldActivator>()
                .As<IActivator>()
                .InField()
                .Singleton()
            );

            Configure(c => c
                .Export<PropertyActivator>()
                .As<IActivator>()
                .InProperty()
                .Singleton()
            );

            Configure(c => c
                .Export<MethodActivator>()
                .As<IActivator>()
                .InMethod()
                .Singleton()
            );

            Configure(c => c
                .Export<ConstructorActivator>()
                .As<IActivator>()
                .InConstructor()
                .Singleton()
            );
        }
    }
}