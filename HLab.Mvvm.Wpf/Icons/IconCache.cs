using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Windows;

namespace HLab.Mvvm.Icons
{
    class IconCache
    {
        private readonly ConcurrentDictionary<string, UIElement> _cache = new ConcurrentDictionary<string, UIElement>();
        private readonly Assembly _assembly;

        public IconCache(Assembly assembly)
        {
            _assembly = assembly;
        }

        public UIElement Get(string name, Func<Assembly, string, UIElement> f)
        {
                return _cache.GetOrAdd(name,n => f(_assembly, n));
        }
    }
}