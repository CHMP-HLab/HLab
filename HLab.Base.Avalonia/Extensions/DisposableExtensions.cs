#nullable enable
using HLab.Base.Disposables;

namespace HLab.Base.Avalonia.Extensions;

public static class DisposableExtensions
{
    public static T AddToDispose<T>(this T @this, DisposeHelper helper)
        where T : IDisposable
    {
        helper.Add(@this);
        return @this;
    }
    public static T DisposeWith<T>(this T @this, ReactiveModel model)
        where T : IDisposable
    {
        model.Disposer.Add(@this);
        return @this;
    }
}