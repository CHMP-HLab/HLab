using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace HLab.Notify.PropertyChanged.PropertyHelpers;

/// <summary>
/// Hold 
/// </summary>
/// <typeparam name="TClass"></typeparam>
[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
public static class PropertyCache<TClass>
    where TClass : class, INotifyPropertyChangedWithHelper
{

    static PropertyCache()
    {
        var type = typeof(TClass).BaseType;
        if (type == null || !typeof(INotifyPropertyChangedWithHelper).IsAssignableFrom(type)) return;

        var t = typeof(PropertyCache<>).MakeGenericType(type);
        var getByHolder = t.GetMethod("GetByHolder", new[] {typeof(string)});
        Debug.Assert(getByHolder!=null);
        GetByHolderParent = s => (PropertyActivator)getByHolder.Invoke(null,new object[]{s});
    }

    static readonly Func<string,PropertyActivator> GetByHolderParent;
    static readonly ConcurrentDictionary<string, PropertyActivator> CacheByHolder = new();
    static readonly ConcurrentDictionary<string, PropertyActivator> CacheByProperty = new();
    public static PropertyActivator GetByProperty(string name )
    {
        if(CacheByProperty.TryGetValue(name, out var e)) return e;
        throw new Exception("Error");
    }

    public static PropertyActivator GetByHolder(string name ) 
        => CacheByHolder.TryGetValue(name, out var e) ? e : GetByHolderParent?.Invoke(name);

    public static PropertyActivator GetByHolder<T>(string name,
        Func<string,NotifyConfigurator<TClass,T>.Activator> activator)
        where T : class,IChildObject
    {
        return CacheByHolder.GetOrAdd(name,valueFactory: n =>
        {
            var a = activator(n);

            return CacheByProperty.GetOrAdd(a.PropertyName,n2 => a);
        });
    }
}