using System;

namespace HLab.Base.Extensions;

public static class FluentConfigureExt
{
    public static T FluentAction<T>(this T target, Action<T> action)
    {
        action(target);
        return target;
    }
}