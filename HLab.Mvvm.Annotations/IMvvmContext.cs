using System;
using HLab.DependencyInjection.Annotations;

namespace HLab.Mvvm.Annotations
{
    public interface IMvvmContext
    {
        void CallCreators(object linked);
        IView GetView(object baseObject, Type viewMode, Type viewClass);
        IView GetView(object baseObject);
        object GetLinked(object o, Type viewMode, Type viewClass);
        IMvvmContext AddCreator<T>(Action<T> action);
        IMvvmContext GetChildContext(string name);
        IMvvmService Mvvm { get; }
        IExportLocatorScope Scope { get; }

        T Locate<T>(object baseObject = null);
        T Locate<T>(Func<T> locate, object baseObject = null);
        object Locate(Type type, object baseObject = null);
    }
}