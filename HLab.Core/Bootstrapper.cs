using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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


        public DependencyInjectionContainer Scope { get; } = new DependencyInjectionContainer();


        private readonly ConcurrentQueue<Context> _queue = new();
        public void Boot()
        {
            var bootLoaders = Sort(Scope.Locate<IEnumerable<IBootloader>>(this));

            HashSet<string> done = new();

            foreach (var bootLoader in bootLoaders)
            {
                var name = bootLoader.GetType().FullName;
                if (done.Contains(name)) continue;
                Enqueue(name, bs => bootLoader.Load(bs));
                done.Add(name);
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
            }
        }

        public void Export<T>()
        {
            if(typeof(T).IsInterface)
            {
                Scope.Configure(c =>
                {
                    var list = ReferencingAssemblies(typeof(T));
                    
                    foreach (var a in list)
                    {
                        var types = a.ExportedTypes.Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(T)));
                        foreach(var type in types)
                        {
                            c
                            .Export(type)
                            .IfNotRegistered(type)
                            .ImportMembers(MembersThat.HaveAttribute<ImportAttribute>(),includeMethods:true)
                            .As(typeof(T));
                        }
                    }
                });
            }
            else
            {
            }
        }
        public void Export<T>(Type generic)
        {
            if (generic.IsInterface && generic.IsGenericType)
            {
                Scope.Configure(c =>
                {
                    var list = ReferencingAssemblies(typeof(T));

                    foreach (var a in list)
                    {
                        var types = a.ExportedTypes.Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(T)));
                        foreach (var type in types)
                        {
                            var generics = type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == generic);
                            foreach (var t in generics)
                            {
                                c
                                .Export(type)
                                .ImportMembers(MembersThat.HaveAttribute<ImportAttribute>(), includeMethods: true)
                                .As(t);
                            }
                        }
                    }
                });
            }
            else
            {
            }
        }

        public void LoadModules()
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

            //AddReference<ImportAttribute>();

            //var assemblies = ReferencingAssemblies();

            Scope.Configure(c => c
                .ImportMembers(MembersThat.HaveAttribute<ImportAttribute>()
                ,true)
                );


            Export<IBootloader>();

            var list = ReferencingAssemblies(typeof(ExportAttribute));

            Scope.Configure(c => c
                .ExportAssemblies(list)
                .ExportAttributedTypes()
            //    .ByInterfaces()

            );
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
        public IEnumerable<Assembly> ReferencingAssemblies(Type type)
        {
            var list = AssemblyHelper.GetReferencingAssemblies(type.Assembly).SortByReferences().ToList();

            return list;
        }

    }
}
