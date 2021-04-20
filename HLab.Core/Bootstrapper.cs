using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Attributes;
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
            private readonly Entry _entry;

            public Context(Bootstrapper bootstrapper, Entry entry)
            {
                _bootstrapper = bootstrapper;
                _entry = entry;
            }

            public void Requeue()
            {
                _bootstrapper.Enqueue(_entry.Name, _entry.Action);
            }

            public void Enqueue(string name, Action<IBootContext> action)
            {
                _bootstrapper.Enqueue(name,action);
            }

            public bool Contains(string name) => _bootstrapper.Contains(name);
        }


        private bool _configured = false;
        private IExportLocatorScope _scope;

        public void Configure(DependencyInjectionContainer  container)
        {
            // TODO : Container.AutoConfigure<IBootloader>(c => c.As<IBootloader>());
            _scope = container;
            LoadModules(container);

            // TODO : Container.ExportInitialize<IInitializer>((c, a, o) => o.Initialize(c, a));

            _configured = true;
        }

        private readonly struct Entry
        {
            public readonly string Name;
            public readonly Action<IBootContext> Action;

            public Entry(string name, Action<IBootContext> action)
            {
                Name = name;
                Action = action;
            }

            public override string ToString() => Name;
        }

        private readonly ConcurrentQueue<Entry> _queue = new();
        public void Boot()
        {
            //if(!_configured) Configure();

            var bootLoaders = SortBootloaders(Sort(_scope.Locate<IEnumerable<IBootloader>>(this))).Reverse().ToList();

            foreach (var bootLoader in bootLoaders)
            {
                Enqueue(bootLoader.GetType().Name, bs => bootLoader.Load(bs));
            }

            while ( _queue.TryDequeue(out var entry) )
            {
                entry.Action(new Context(this,entry));
            }
        }

        private static IEnumerable<T> Sort<T>(IEnumerable<T> src)
        {
            var result = new List<T>();
            foreach (var boot in src)
            {
                var inserted = false;
                for (var i = 0; i < result.Count; i++)
                {
                    if (boot.GetType().Assembly.References(result[i].GetType().Assembly.GetName().Name))
                    {
                        result.Insert(i, boot);
                        inserted = true;
                        break;
                    }
                }
                if (!inserted) result.Add(boot);
            }

            return result;
        }
        private static IEnumerable<IBootloader> SortBootloaders(IEnumerable<IBootloader> src)
        {
            var result = new List<IBootloader>();
            foreach (var boot in src)
            {
                var inserted = false;
                if(boot is IBootloaderDependent bd)
                    for (var i = 0; i < result.Count; i++)
                    {
                        if (bd.DependsOn.Contains(result[i].GetType().Name))
                        {
                            result.Insert(i, boot);
                            inserted = true;
                            break;
                        }
                    }
                if (!inserted) result.Add(boot);
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
            catch(FileNotFoundException )
            {}
            catch (BadImageFormatException)
            {
            } 
            return false;
        }

        private void LoadModules(DependencyInjectionContainer container)
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

            container.Configure(c => c
                .ExportAssemblies(assemblies).ExportAttributedTypes().ByInterface<IBootloader>()
            );

            //container.Configure(c => c
            //    .ExportAssemblies(assemblies).ByInterface<IBootloader>()
            //);


            // TODO : Container.StaticInjection();

        }

        public void Enqueue(string name,Action<IBootContext> action)
        {
            _queue.Enqueue(new Entry(name, action));
        }

        public bool Contains(string name)
        {
            return _queue.Any(e => e.Name == name);
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
