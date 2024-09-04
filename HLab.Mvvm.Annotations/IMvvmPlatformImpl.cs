using System;
using System.Threading;
using System.Threading.Tasks;

namespace HLab.Mvvm.Annotations;

public interface IMvvmPlatformImpl
{
    /// <summary>
    /// Provide a default view 
    /// </summary>
    /// <param name="getType"></param>
    /// <param name="viewMode"></param>
    /// <param name="viewClass"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<IView> GetNotFoundViewAsync(Type getType, Type viewMode, Type viewClass, CancellationToken token = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="view"></param>
    /// <param name="token"></param>
    Task PrepareViewAsync(IView view, CancellationToken token = default);

    /// <summary>
    /// 
    /// </summary>
    void Register(IMvvmService mvvm);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    void Register(Type type);


    /// <summary>
    /// Called when a view is activated
    /// </summary>
    /// <param name="obj"></param>
    object Activate(IView obj);

    object Deactivate(IView obj);

}