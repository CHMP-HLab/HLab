using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Windows;

namespace HLab.Icons.Wpf.Icons;

internal class IconCache
{
    readonly ConcurrentDictionary<string, UIElement> _cache = new();
    readonly Assembly _assembly;

    public IconCache(Assembly assembly)
    {
        _assembly = assembly;
    }

    public UIElement Get(string name, Func<Assembly, string, UIElement> f)
    {
        return _cache.GetOrAdd(name,n => f(_assembly, n));
    }
}