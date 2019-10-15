using System;
using System.Reflection;

namespace HLab.DependencyInjection.Annotations
{
    public interface IExportLocatorScope
    {
        T Locate<T>(object target=null);
        T Locate<T>(object target, IImportContext ctx);
        object Locate(Type type, object target = null);
        object Locate(IRuntimeImportContext ctx, object[] args = null);
        void Inject(object obj, object[] args, IRuntimeImportContext ctx);

        IExportLocatorScope Configure(Func<IConfigurator, IConfigurator> configurator);
        DependencyInjector GetClassInjector(IActivatorTree tree, InjectLocation location);
        IExportLocatorScope AutoConfigure<T>(Func<IConfigurator, IConfigurator> configurator);

        void ExportAssembly(Assembly assembly);
        void ExportInitialize<T>(Action<IRuntimeImportContext, object[], T> action);
    }
}