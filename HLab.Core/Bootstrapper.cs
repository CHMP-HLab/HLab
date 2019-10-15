using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using HLab.Base;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;

namespace HLab.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class Bootstrapper
    {
        public IExportLocatorScope Container { get; }

        [Import] public Bootstrapper(IExportLocatorScope container)
        {
            Container = container;
        }
        public void Boot()
        {
//            Container.AutoConfigure<IService>(c => c.As<IService>().Singleton());
            Container.AutoConfigure<IBootloader>(c => c.As<IBootloader>());
            Container.AutoConfigure<IPostBootloader>(c => c.As<IPostBootloader>());
            Container.AutoConfigure<IConfigureInjection>(c => c.As<IConfigureInjection>());

            LoadModules();

            Container.ExportInitialize<IInitializer>((c, a, o) => o.Initialize(c, a));

            foreach (var injection in Container.Locate<IEnumerable<IConfigureInjection>>(this))
            {
                injection.Configure(Container);
            }

            var prebootloaders = Sort(Container.Locate<IEnumerable<IBootloader>>(this)).Reverse().ToList();
            var postbootloaders = Sort(Container.Locate<IEnumerable<IPostBootloader>>(this)).Reverse().ToList();

            foreach (var boot in prebootloaders)
            {
                boot.Load();
            }
            foreach (var boot in postbootloaders)
            {
                boot.Load();
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
                    if (boot.GetType().Assembly.Referencies(result[i].GetType().Assembly.GetName().Name))
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

        private static IEnumerable<Assembly> Sort(IEnumerable<Assembly> src)
        {
            var result = new List<Assembly>();
            foreach (var assembly in src)
            {
                var inserted = false;
                for (var i = 0; i < result.Count; i++)
                {
                    if (assembly.Referencies(result[i].GetName().Name))
                    {
                        result.Insert(i,assembly);
                        inserted = true;
                        break;
                    }
                }
                if(!inserted) result.Add(assembly);
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

                //var assembly = Assembly.LoadFile(path);
                var assembly = Assembly.Load(path);

                foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
                {

                    LoadDll(referencedAssembly.Name);
                }

                return true;
            }
            catch (BadImageFormatException)
            {
            } 
            return false;
        }

        private void LoadModules()
        {
            // var all = AppDomain.CurrentDomain.GetAssemblies();
            //foreach (var assembly in all)
            //{
            //    if(!_loadedAssemblies.ContainsKey(assembly.Location))
            //        _loadedAssemblies.Add(assembly.Location,assembly);
            //}

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




//            var loadedAssemblies = new Dictionary<string,Assembly>();

            //var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assemblies = Sort(AssemblyHelper.GetReferencingAssemblies("HLab.DependencyInjection.Annotations"));
            foreach (var assembly in assemblies)
            {
                Container.ExportAssembly(assembly);
            }

            //var directory = AppDomain.CurrentDomain.BaseDirectory;
            //if (directory!=null) // on android 
            //{
            //    var dlls = Directory.GetFiles(directory, "*.Module.dll");
            //    foreach (var f in dlls)
            //    {
            //        if (loadedAssemblies.ContainsKey(f)) continue;
            //        try { 
            //            var assembly = Assembly.LoadFile(f);
            //            if (loadedAssemblies.ContainsValue(assembly)) continue;
            //            loadedAssemblies.Add(assembly.Location,assembly);
            //            LoadModule(assembly);
            //        }
            //        catch(BadImageFormatException)
            //        { }
            //    }
            //}
        }
    }
}
