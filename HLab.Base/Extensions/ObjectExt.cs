using System;

namespace HLab.Base.Extensions;

public static class ObjectExt
{
    public static T Cast<T>(this object o)
    {
        switch (o)
        {
            case T r:
                return r;
            case null:
                return default(T);
            default:
                throw new InvalidCastException(o.GetType().Name + " => " + typeof(T).Name);
        }
    }
}