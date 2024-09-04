using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HLab.Base;

public static class AssemblyHelper
{
    public static IEnumerable<Assembly> GetReferencingAssemblies(params Assembly[] referenced)
    {
        return GetAssemblies().Where(a => a.References(referenced));
    }
    public static IEnumerable<Assembly> GetReferencingAssemblies(string name)
    {
        return GetAssemblies().Where(a => a.References(name));
    }
    public static IEnumerable<Assembly> GetAssemblies()
    {
        return AppDomain.CurrentDomain.GetAssemblies();
    }

    public static IEnumerable<Assembly> GetReferencingAssemblies(Type type)
    {
        var assembly = Assembly.GetAssembly(type).GetName();
        return GetReferencingAssemblies(assembly.Name);
    }

    public static bool References(this Assembly assembly, params Assembly[] referenced) => referenced.Any(r => assembly.References(r.GetName().Name));

    public static bool References(this Assembly assembly, string name)
    {
        var references = assembly.GetReferencedAssemblies().Any(e => e.Name == name);
        if (assembly.GetName().Name == "HLab.Erp.Base.Data")
        {
            var test = assembly.GetReferencedAssemblies();
        }

        return references;
    }


    static readonly ConcurrentDictionary<Assembly, Type[]> _cache = new();
    public static Type[] GetTypesSafe(this Assembly assembly)
    {
        try
        {
            return _cache.TryGetValue(assembly,out var t) ? t : assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            return _cache.GetOrAdd(assembly,a =>
                e.Types.Where(t => t != null /*&& t.TypeInitializer !=null*/).ToArray());;
        }
    }

    public static IEnumerable<Assembly> SortByReferences(this IEnumerable<Assembly> src)
    {
        var result = new List<Assembly>();
        foreach (var assembly in src)
        {
            var inserted = false;
            for (var i = 0; i < result.Count; i++)
            {
                if (assembly.References(result[i].GetName().Name))
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


}