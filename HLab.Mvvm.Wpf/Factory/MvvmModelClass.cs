using System;
using System.Collections.Concurrent;

namespace HLab.Mvvm.Factory
{
    internal static class MvvmClasses
    {
        public const int Default = 0;
        public const int Content = Default + 1;
    }

    internal class MvvmClass : Attribute
    {
        public object Value { get; }
        public MvvmClass(object value)
        {
            Value = value;
        }
    }

    internal class MvvmModelClass
    {
        ConcurrentDictionary<LinkKey, MvvmLink> _viewClasses = new ConcurrentDictionary<LinkKey, MvvmLink>();

        public MvvmLink GetLink(Type viewClass, Type viewMode) => GetLink(new LinkKey(viewClass, viewMode));
        public MvvmLink GetLink(LinkKey key)
        {
            return _viewClasses.GetOrAdd(key, GetNearestLink);
        }

        MvvmLink GetNearestLink(LinkKey key)
        {
            MvvmLink result = null;
            LinkKey bestKey = null;
            foreach (var link in _viewClasses)
            {
                var k = link.Key;
                if (key.IsAssignableFrom(k))
                {
                    //La cle demandée est plus précise que la clé stockée

                    if (bestKey == null || bestKey.IsAssignableFrom(k))
                    {
                        bestKey = k;
                        result = link.Value;
                    }
                }
                if (k.IsAssignableFrom(key))
                {
                    // la clé stockée est plus précise que la clé demandée
                    if (bestKey == null || bestKey.IsAssignableFrom(k))
                    {
                        bestKey = k;
                        result = link.Value;
                    }
                }
            }
            return result;
        }
    }
}
