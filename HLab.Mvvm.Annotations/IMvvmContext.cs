using System;
using System.Threading;
using System.Threading.Tasks;

namespace HLab.Mvvm.Annotations;

public interface IMvvmContext
{
    void CallCreators(object linked);

    Task<IView?> GetViewAsync(object baseObject, Type viewMode, Type viewClass, CancellationToken token = default);
    Task<IView?> GetViewAsync(object baseObject, CancellationToken token = default);
    Task<object> GetLinkedAsync(object o, Type viewMode, Type viewClass, CancellationToken token = default);

    IMvvmContext AddCreator<T>(Action<T> action);
    IMvvmContext GetChildContext(string name);
    IMvvmService Mvvm { get; }

    T Locate<T>(object baseObject = null);
    T Locate<T>(Func<T> locate, object baseObject = null);
    object Locate(Type type, object baseObject = null);
}