using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using HLab.Base;
using HLab.DependencyInjection.Activators;
using HLab.DependencyInjection.Annotations;
// ReSharper disable UnusedMember.Local

namespace HLab.DependencyInjection
{


    public class DependencyInjectionContainer : IExportLocatorScope
    {
        public IExportLocatorScope Configure(Func<IConfigurator,IConfigurator> configurator)
        {
            if (configurator(new Configurator(this)) is not Configurator c) return this;

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
            return _autoConfigure.TryAdd(typeof(T), configurator) ? AddReference<T>() : this;
        }
        public IExportLocatorScope AddReference<T>()
        {
            _referencesAssemblies.Add(typeof(T).Assembly);
            return this;
        }

        private readonly ConcurrentDictionary<Type, Func<IConfigurator, IConfigurator>> _autoConfigure = new();

        private readonly ConcurrentDictionary<IImportContext, IDependencyLocator> _locators = new();

        private readonly WeightedList<IExportEntry> _locatorEntries = new WeightedList<IExportEntry>().AddComparator((a,b) => a.Value.Priority - b.Value.Priority);
        private readonly List<IExportEntry> _decoratorEntries = new();
        private readonly ConcurrentHashSet<Assembly> _referencesAssemblies = new();


        private readonly ConcurrentDictionary<Type, DependencyInjectorSet> _injectors
            = new();

        internal readonly ConcurrentDictionary<Type, object> Singletons = new ConcurrentDictionary<Type, object>();
        //private readonly ConcurrentDictionary<Type,List<IMappedCondition>> _mapped = new ConcurrentDictionary<Type, List<IMappedCondition>>();
        private readonly ConcurrentDictionary<Type, List<DependencyInjector>> _initializers = new();

        public void ExportReferencingAssemblies()
        {
            var assemblies = AssemblyHelper.GetReferencingAssemblies(_referencesAssemblies.ToList().ToArray()).SortByReferences();
            foreach (var assembly in assemblies)
            {
                ExportAssembly(assembly);
            }
        }

        public void ExportInitialize<T>(Action<object[], T> action)
        {
            Initializer(typeof(T),(a,t)=>action(a,(T)t));

        }
        public void Initializer(Type type, DependencyInjector action)
        {
            var list = _initializers.GetOrAdd(type, _ => new List<DependencyInjector>());
            list.Add(action);
        }

        public void StaticInjection() => _staticInjection?.Invoke();
        private Action _staticInjection;
             
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

                const BindingFlags flag = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

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


        public object Locate(Type type) 
            => Locate(ImportContext.Create(type));


        public T Locate<T>(object target = null) 
            => (T)Locate(RuntimeImportContext.GetStatic(target, typeof(T)));

        public T Locate<T>(IImportContext ctx)
            => (T)Locate(RuntimeImportContext.GetStatic(target, ctx));

        public object Locate(Type type, object[]args = null) 
            => GetLocator(new ActivatorTree(null,ctx)).Locate(args);

        private IDependencyLocator GetLocator(IActivatorTree tree) 
            => _locators.GetOrAdd(tree.Context, _ => GetNewLocator<object>(tree));

        private IDependencyLocator<T> GetLocator<T>(IActivatorTree tree) 
            => (IDependencyLocator<T>)_locators.GetOrAdd(tree.Context, _ => GetNewLocator<T>(tree));


        public void Inject(object obj, object[] args, IImportContext ctx)
        {
            var tree = new ActivatorTree(null, ctx)
            {
                Key = new ActivatorKey(obj.GetType(), null)
            };
            var injectors = GetClassInjector(tree);
            injectors.Constructor(args, obj);
            injectors.PostConstructor(args, obj);
        }

        private class EmptyEnumerable<T> : IEnumerable<T>
        {
            private class EmptyEnumerator : IEnumerator<T>
            {
                public bool MoveNext() => false;

                public void Reset() { }

                public T Current => throw new IndexOutOfRangeException();

                object IEnumerator.Current => Current;

                public void Dispose() { }
            }

            public IEnumerator<T> GetEnumerator() => new EmptyEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => new EmptyEnumerator();
        }

        private DependencyLocator<IEnumerable<T>> GetNewLocatorIEnumerable1<T>(IActivatorTree tree)
        {
            Action<object[], List<T>> action = null;

            var testTree = new ActivatorTree(tree, tree.Context.CreateChild<T>());

            foreach (var e in _locatorEntries.Where(l => l.Test(testTree)).ToList())
            {
                var activator = e.Locator(new ActivatorTree(tree, tree.Context.CreateChild<T>()));

                action += (a,l) => l.Add((T)activator.Locate(a));
            }

            if (action == null) return new DependencyLocator<IEnumerable<T>>((_) => new EmptyEnumerable<T>());

            return new DependencyLocator<IEnumerable<T>>(a =>
            {
                var l = new List<T>();
                action(a,l);
                return l;
            });
        }

        private DependencyLocator<Lazy<T>> GetNewLocatorLazy1<T>(ActivatorTree tree)
        {
            tree = new ActivatorTree<T>(tree,tree.Context.CreateChild<T>());

            var locator = GetLocator(tree);
            return new DependencyLocator<Lazy<T>>(a => new Lazy<T>(() => (T)locator.Locate(a)));
        }

        private DependencyLocator<Func<T>> GetNewLocatorFunc1<T>(ActivatorTree tree)
        {
            tree = new ActivatorTree(tree, tree.Context.CreateChild<T>());
            var locator = GetLocator(tree);

            return new DependencyLocator<Func<T>>(
                (a) => 
                    () => (T)locator.Locate(a));
        }

        private DependencyLocator<Func<TArg,TResult>> GetNewLocatorFunc2<TArg, TResult>(ActivatorTree tree)
        {
            var childTree = new ActivatorTree(tree, tree.Context.CreateChild<TResult>(new MethodSignature(typeof(TArg))));
            var locator = GetLocator(childTree);
            return new DependencyLocator<Func<TArg, TResult>>(
                (_) => 
                    t => (TResult)locator.Locate(new object[] { t })
                    );
        }
        private DependencyLocator<Func<T1, T2, TResult>> GetNewLocatorFunc3<T1, T2, TResult>(ActivatorTree tree)
        {
            tree = new ActivatorTree(tree, tree.Context.CreateChild(typeof(TResult), new MethodSignature(typeof(T1), typeof(T2))));
            var locator = GetLocator(tree);
            return new DependencyLocator<Func<T1, T2, TResult>>(
                ( _) => 
                    (t1, t2) => (TResult)locator.Locate(new object[] { t1, t2 })
                    );
        }
        private DependencyLocator<Func<T1, T2, T3, TResult>> GetNewLocatorFunc4<T1, T2, T3, TResult>(ActivatorTree tree)
        {
            tree = new ActivatorTree(tree, tree.Context.CreateChild(typeof(TResult), new MethodSignature(typeof(T1), typeof(T2), typeof(T3))));
            var locator = GetLocator(tree);
            return new DependencyLocator<Func<T1, T2, T3, TResult>>(
                (_) => 
                    (t1, t2, t3) => (TResult)locator.Locate(new object[] { t1, t2, t3 })
                    );
        }
        private DependencyLocator<Func<T1, T2, T3, T4, TResult>> GetNewLocatorFunc5<T1, T2, T3, T4, TResult>(ActivatorTree tree)
        {
            tree = new ActivatorTree(tree, tree.Context.CreateChild(typeof(TResult), new MethodSignature(typeof(T1), typeof(T2), typeof(T3), typeof(T4))));
            var locator = GetLocator(tree);
            return new DependencyLocator<Func<T1, T2, T3, T4, TResult>>(
                (_) => 
                    (t1, t2, t3, t4) => (TResult)locator.Locate(new object[] { t1, t2, t3, t4 })
                    );
        }

        private IDependencyLocator<T> GetNewLocator<T>(IActivatorTree tree)
        {
            var importType = tree.Context.ImportType;

            if (importType.IsGenericType)
            {
                var g = importType.GetGenericTypeDefinition();
                var args = importType.GetGenericArguments();

                // return a Func<Type,object> to provide generic locator without arguments
                if (args.Length==2 && args[0] == typeof(Type) && args[1] == typeof(object))
                    return (IDependencyLocator<T>)new DependencyLocator<Func<Type, object>>(
                        (_) => 
//                            t => Locate(c.NewChild(c.Target, tree.Context.CreateChild(t))));
                            t => Locate(t));

                //return a Func<Type,object[],object> to provide a generic locator with arguments
                if (args.Length == 3 && args[0] == typeof(Type) && args[1] == typeof(object[]) && args[2] == typeof(object))
                    return (IDependencyLocator<T>)new DependencyLocator<Func<Type, object[], object>>((_) => (t,ar) =>
                    {
                        var types = new MethodSignature(ar);
//                        return Locate(c.NewChild(c.Target, tree.Context.CreateChild(t,types)), ar);
                        return Locate(t,ar);
                    });

                var name = g.Name.Split('`')[0];

                var mi = GetType().GetMethod($"GetNewLocator{name}{args.Length}",BindingFlags.NonPublic|BindingFlags.Instance);
                if(mi != null)
                {
                    var m = mi.MakeGenericMethod(args);

                    var r = m.Invoke(this, new object[] { tree });

                    if (r is IDependencyLocator<T> l) return l;

                    throw new Exception("");
                }

            }
            
            {
                var decorators = _decoratorEntries.Where(e => e.Test(tree));

                var exportEntry = _locatorEntries.Get(e => e.Test(tree));

                IActivatorKey export;
                ExportMode mode;

                // when no export entry found but type is instantiable use it TODO : should be an option
                if (exportEntry == null)
                {
                    if (!importType.IsAbstract && !importType.IsInterface)
                    {
                        exportEntry = new ExportEntry(this)
                        {
                            ExportType = _ => importType
                        };

                        export = new ActivatorKey(importType,tree.Context.Signature);
                        mode = ExportMode.Default;
                    }
                    else
                    {
                        //var t = new ActivatorTree(null,new ImportContext());
                        //var n = _locatorEntries.Get(e =>
                        //{
                        //    try
                        //    {
                        //        var exportType = e.ExportType(t);
                        //        return importType.IsAssignableFrom(exportType);
                        //    }
                        //    catch
                        //    {

                        //        return false;
                        //    }
                        //});

                        throw new Exception("Unable to locate " + tree.ToString() /*importType.GenericReadableName()*/);
                    }
                }
                else
                {
                    export = exportEntry.GetActivatorKey(tree);
                    mode = exportEntry.Mode;
                }

                IDependencyLocator<T> activator;
                if (mode.HasFlag(ExportMode.Singleton))
                {
                    var type = export.ReturnType;

                    tree.Key = new ActivatorKey(type, null);

                    var singleton = Singletons.GetOrAdd(type, _ => exportEntry.Locator(tree).Locate(null));

                    activator = new DependencyLocator<T>((_) => (T)singleton);
                }
                else
                    activator = (IDependencyLocator<T>)exportEntry.Locator(tree);


                foreach (var de in decorators)
                {
                    var decorator = de.Locator(new ActivatorTree(tree,
                        tree.Context.CreateChild(importType, new MethodSignature(importType))));

                    var old = activator;

                    activator = new DependencyLocator<T>((a) => (T)decorator.Locate(new[] { (object)old.Locate(a) }));
                }


                return activator;
            }
        }


        private DependencyInjectorSet GetNewClassInjector(IActivatorTree tree)
        {
            var returnType = tree.Key.ReturnType;

            if (returnType.IsAbstract) throw new Exception("Unable to locate Abstract class : " + returnType.Name);
            if (returnType.IsInterface) throw new Exception("Unable to locate Interface : " + returnType.Name);

            DependencyInjector activator = null;
            List<DependencyInjector> activatorCtor = new();

            var types = new Stack<Type>();
            var t = returnType;


            while (t != null)
            {
                types.Push(t);
                t = t.BaseType;
            }

            while (types.Count > 0)
            {
                t = types.Pop();
                foreach (var memberInfo in t.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
                {
                    if (memberInfo is ConstructorInfo ci)
                    {
                        var context = ImportContext.Create(returnType, ci).CreateChild(typeof(IActivator));
                        var l = GetLocator(new ActivatorTree(tree, context));
                        var a = (IActivator)l.Locate(null);
                        activatorCtor.Add(a.GetActivator(GetLocator, new ActivatorTree(tree, ImportContext.Create(returnType, ci))));
                    }
                    else
                    {
                        foreach (var attr in memberInfo.GetCustomAttributes<ImportAttribute>())
                        {
                            var ctx = ImportContext.Create(returnType, memberInfo).CreateChild(typeof(IActivator));
                            var l = GetLocator(new ActivatorTree(tree, ctx));
                            var a = (IActivator)l.Locate(null);

                            activator += a.GetActivator(GetLocator, new ActivatorTree(tree, ImportContext.Create(returnType, memberInfo)));
                        }
                    }
                }
            }


            foreach (var (key, value) in _initializers)
            {
                if (key.IsAssignableFrom(returnType))
                {
                    foreach (var action in value)
                        activator += action;
                }
            }

            return new DependencyInjectorSet{ Constructor = activatorCtor.FirstOrDefault(), PostConstructor = activator};
        }

        public DependencyInjectorSet GetClassInjector(IActivatorTree tree)
            => _injectors.GetOrAdd(tree.Key.ReturnType, _ => GetNewClassInjector(tree));

        public DependencyInjectionContainer()
        {
            Configure(c => c.Export<DependencyInjectionContainer>().Set(e => e.Locator = _ => new DependencyLocator<object>((_) => this)).As<IExportLocatorScope>());

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