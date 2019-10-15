using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HLab.Base
{
    public static class AssemblyHelper
    {
        public static IEnumerable<Assembly> GetReferencingAssemblies(string name) 
            => AppDomain.CurrentDomain.GetAssemblies().Where(a => a.Referencies(name));

        public static IEnumerable<Assembly> GetReferencingAssemblies(Type type)
        {
            var assembly = Assembly.GetAssembly(type).GetName();
            return GetReferencingAssemblies(assembly.Name);
        }

        public static bool Referencies(this Assembly assembly, string name)
        {
            return assembly.GetReferencedAssemblies().Any(e => e.Name == name);
        }


        private static ConcurrentDictionary<Assembly, Type[]> _cache = new ConcurrentDictionary<Assembly, Type[]>();
        public static Type[] GetTypesSafe(this Assembly assembly)
        {


            try
            {
                if(_cache.TryGetValue(assembly,out var t))
                    return t;

                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                var types =
                    _cache.GetOrAdd(assembly,a =>
                    e.Types.Where(t => t != null /*&& t.TypeInitializer !=null*/).ToArray());

                return types;
            }
        }

    }
}
