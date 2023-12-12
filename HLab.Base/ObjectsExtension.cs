using System;
using System.Linq;

namespace HLab.Base;

public static class ObjectsExtension
{
    class ObjectHelper<T>
    {
        static readonly Action<T, T> CopyPrimitivesToAction = GetCopyTo(true);
        static readonly Action<T, T> CopyToAction = GetCopyTo(false);

        public static void CopyPrimitivesTo(T source, T target)
        {
            if (target != null && source != null && !ReferenceEquals(source, target))
            {
                CopyPrimitivesToAction?.Invoke(source, target);
            }
        }
        public static void CopyTo(T source, T target)
        {
            if (target != null && source != null && !ReferenceEquals(source, target))
            {
                CopyToAction?.Invoke(source, target);
            }
        }

        static Action<T, T> GetCopyTo(bool primitivesOnly)
        {
            Action<T, T> action = null;
            foreach (var info in typeof(T).GetProperties().Where(p => p.CanWrite))
            {
                var t = info.PropertyType;
                if (t.IsConstructedGenericType)
                {
                    if (info.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        t = t.GetGenericArguments()[0];
                    }
                }

                if (!primitivesOnly
                    ||t.IsPrimitive
                    || t == typeof(string)
                    || t == typeof(DateTime)
                    || t == typeof(Byte[])
                   )
                    action += (source, target) =>
                        info.SetValue(target, info.GetValue(source));
            }

            return action;
        }
    }


    public static void CopyPrimitivesTo<T>(this T source, T target)
    {
        ObjectHelper<T>.CopyPrimitivesTo(source, target);
    }
    public static void CopyTo<T>(this T source, T target)
    {
        ObjectHelper<T>.CopyTo(source, target);
    }
}