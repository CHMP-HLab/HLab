using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Attributes;
using Grace.DependencyInjection.Impl.CompiledStrategies;
using HLab.Base;
using HLab.Core.Annotations;

namespace HLab.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class Bootstrapper
    {

        private class Context : IBootContext
        {
            private readonly Bootstrapper _bootstrapper;
            private readonly Action<IBootContext> _action;
            public string Name { get; }

            public Context(Bootstrapper bootstrapper, string name, Action<IBootContext> action)
            {
                _bootstrapper = bootstrapper;
                _action = action;
                Name = name;
            }

            public void Requeue()
            {
                _bootstrapper.Enqueue(this);
            }

            public void Enqueue(string name, Action<IBootContext> action)
            {
                _bootstrapper.Enqueue(name, action);
            }

            public void Invoke() => _action(this);

            public bool StillContains(params string[] name) => _bootstrapper.Contains(name);
            public override string ToString() => Name;
        }


        private DependencyInjectionContainer _scope;

        public void Configure(DependencyInjectionContainer container)
        {
            _scope = container;
            LoadModules();

            // TODO : Container.ExportInitialize<IInitializer>((c, a, o) => o.Initialize(c, a));
        }


        private readonly ConcurrentQueue<Context> _queue = new();
        public void Boot()
        {
            var bootLoaders = Sort(_scope.Locate<IEnumerable<IBootloader>>(this));

            foreach (var bootLoader in bootLoaders)
            {
                Enqueue(bootLoader.GetType().Name, bs => bootLoader.Load(bs));
            }

            while (_queue.TryDequeue(out var context))
            {
                context.Invoke();
            }
        }

        private static IEnumerable<T> Sort<T>(IEnumerable<T> src)
        {
            var result = new List<T>();
            foreach (var boot in src)
            {
                var bootAssemblyName = boot.GetType().Assembly.GetName().Name;
                for (var i = 0; i < result.Count; i++)
                {
                    var a = result[i].GetType().Assembly;
                    if (a.References(bootAssemblyName))
                    {
                        result.Insert(i, boot);
                        goto inserted;
                    }
                }
                result.Add(boot);
            inserted:;
            }

            return result;
        }




        //private readonly Dictionary<string, Assembly> _loadedAssemblies = new Dictionary<string, Assembly>();



        public bool LoadDll(string name)
        {
            return LoadAbsolutePath(name);
            //var path = AppDomain.CurrentDomain.BaseDirectory + name +".dll";
            //return LoadAbsolutePath(path);
        }

        private bool LoadAbsolutePath(string path)
        {
            //if (!File.Exists(path)) return false;

            if (AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name == path)) return false;

            try
            {
                var assembly = Assembly.Load(path);

                foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
                {

                    LoadDll(referencedAssembly.Name);
                }

                return true;
            }
            catch (FileNotFoundException)
            { }
            catch (BadImageFormatException)
            {
            }
            return false;
        }

        class strategy : IActivationStrategyInspector
        {
            public void Inspect<T>(T strategy) where T : class, IActivationStrategy
            {
                throw new NotImplementedException();
            }
        }

        public void Export<T>()
        {
            _scope.Configure(c =>
            {
                c
                    .ExportAssemblies(
                        ReferencingAssemblies())
                    .Where(y => typeof(T).IsAssignableFrom(y))
                    .ByInterfaces().ByType().ExportAttributedTypes();
            });
        }

        private void LoadModules()
        {

            var directory = AppDomain.CurrentDomain.BaseDirectory;
            if (directory != null) // on android 
            {
                var dlls = Directory.GetFiles(directory, "*.Module.dll");
                foreach (var path in dlls)
                {
                    var filename = Path.GetFileNameWithoutExtension(path);
                    LoadAbsolutePath(filename);
                }
            }

            AddReference<ImportAttribute>();

            var assemblies = ReferencingAssemblies();

            _scope.Configure(c => c
                .ExportAssemblies(assemblies).ExportAttributedTypes().ByInterface<IBootloader>()
            );

            //container.Configure(c => c
            //    .ExportAssemblies(assemblies).ByInterface<IBootloader>()
            //);


            // TODO : Container.StaticInjection();

        }
        private void Enqueue(Context context)
        {
            _queue.Enqueue(context);
        }

        public void Enqueue(string name, Action<IBootContext> action)
        {
            Enqueue(new Context(this, name, action));
        }

        public bool Contains(params string[] names)
        {
            return names.Any(name => _queue.Any(e => e.Name == name));
        }

        private readonly ConcurrentHashSet<Assembly> _referencesAssemblies = new();
        public IEnumerable<Assembly> ReferencingAssemblies()
        {
            return AssemblyHelper.GetReferencingAssemblies(_referencesAssemblies.ToList().ToArray()).SortByReferences();
        }
        public void AddReference<T>()
        {
            _referencesAssemblies.Add(typeof(T).Assembly);
        }

    }
}
