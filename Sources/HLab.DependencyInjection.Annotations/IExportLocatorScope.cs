using System;
using System.Reflection;

namespace HLab.DependencyInjection.Annotations
{
    public interface IExportLocatorScope
    {
        T Locate<T>();
        T Locate<T>(IImportContext ctx);
        object Locate(Type type);
        object Locate(object[] args);
        void Inject(object[] args);

        IExportLocatorScope Configure(Func<IConfigurator, IConfigurator> configurator);
        DependencyInjectorSet GetClassInjector(IActivatorTree tree);
        IExportLocatorScope AutoConfigure<T>(Func<IConfigurator, IConfigurator> configurator);
        IExportLocatorScope AddReference<T>();

        void ExportAssembly(Assembly assembly);
        void ExportReferencingAssemblies();
        void ExportInitialize<T>(Action<object[], T> action);

        void StaticInjection();
    }
}