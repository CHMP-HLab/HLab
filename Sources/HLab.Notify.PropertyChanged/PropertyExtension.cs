using System;
using System.Collections.Generic;
using HLab.Notify.PropertyChanged.PropertyHelpers;

namespace HLab.Notify.PropertyChanged
{
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
//            where T : class
            where TClass : class
        {
            if (property is PropertyHolder<T> p)
            {
                 if (p.Parent is TClass target)
                 {
                    var oldValue = property.Get();

                    if (property.Set(value))
                    {
                        if (oldValue != null) getCollection(oldValue).Remove(target);
                        if (value != null) getCollection(value).Add(target);
                        return true;
                    }
                 }
            }

            return false;
        }
    }
}