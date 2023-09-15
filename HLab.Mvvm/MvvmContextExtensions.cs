using System;
using System.Threading;
using System.Threading.Tasks;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm;

public static class MvvmContextExtensions
{
    public static Task<IView?> GetViewAsync(this IMvvmContext ctx, object baseObject, Type viewMode, CancellationToken token = default)
        => ctx.GetViewAsync(baseObject, viewMode, typeof(IDefaultViewClass), token);
        
    public static Task<IView?> GetViewAsync<T>(this IMvvmContext ctx, object baseObject, Type viewClass, CancellationToken token = default)
        => ctx.GetViewAsync(baseObject, typeof(T), viewClass, token);
        
    public static Task<IView?> GetViewAsync<T>(this IMvvmContext ctx, object baseObject, CancellationToken token = default)
        => ctx.GetViewAsync(baseObject, typeof(T), typeof(IDefaultViewClass), token);
        
    public static Task<IView?> GetViewAsync<TMode,TClass>(this IMvvmContext ctx, object baseObject, CancellationToken token = default)
        => ctx.GetViewAsync(baseObject, typeof(TMode), typeof(TClass), token);
        
    public static Task<object> GetLinkedAsync<T>(this IMvvmContext ctx, object o, Type viewClass, CancellationToken token = default) 
        => ctx.GetLinkedAsync(o, typeof(T), viewClass, token);

    public static Task<object> GetLinkedAsync<T>(this IMvvmContext ctx, object o, CancellationToken token = default) 
        => ctx.GetLinkedAsync(o, typeof(T), typeof(IDefaultViewClass), token);

    public static Task<object> GetLinkedAsync<TMode,TClass>(this IMvvmContext ctx, object o, CancellationToken token = default) 
        => ctx.GetLinkedAsync(o, typeof(TMode), typeof(TClass), token);
}