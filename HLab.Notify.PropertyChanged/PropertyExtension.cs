using System;
using System.Collections.Generic;
using HLab.Notify.PropertyChanged.PropertyHelpers;

namespace HLab.Notify.PropertyChanged;

public static class PropertyExtension
{
    public static bool Set<T>(this IProperty<T> t, T value, Action<T,T> todo)
    {
        var old = t.Get();
        if(t.Set(value))
        {
            todo(old, value);
            return true;
        }
        return false;
    }
    public static bool Set<T>(this IProperty<T> t, T value, Action<T> todo)
    {
        if(t.Set(value))
        {
            todo(value);
            return true;
        }
        return false;
    }
    public static bool Set<T>(this IProperty<T> t, T value, Action todo)
    {
        if (t.Set(value))
        {
            todo();
            return true;
        }
        return false;
    }
    public static bool SetOneToMany<T, TClass>(this IProperty<T> property, T value, Func<T, IList<TClass>> getCollection)
        where TClass : class
    {
        if (property is not PropertyHolder<T> p) return false;
            
        if (p.Parent is not TClass target) return false;
            
        var oldValue = property.Get();

        if (!property.Set(value)) return false;
                    
        if (oldValue != null)
        {
            var collection = getCollection(oldValue);
            collection?.Remove(target);
        }
                    
        if (value != null)
        {
            var collection = getCollection(value);
            collection?.Add(target);
        }
                    
        return true;
    }
}