using System;

namespace HLab.DependencyInjection.Annotations
{
    public delegate void DependencyInjector(IRuntimeImportContext ctx, object[] args, object target);
    public delegate object DependencyLocator(IRuntimeImportContext ctx, object[] args);

    public interface IExportEntry
    {
        IExportLocatorScope Scope { get; }
        void AndCondition(Func<IActivatorTree, bool> condition);
        void OrCondition(Func<IActivatorTree, bool> condition);
        Func<IRuntimeImportContext, bool> RuntimeCondition { get; set; }
        //DependencyLocator GetActivator(IActivatorTree tree);
        IActivatorKey GetActivatorKey(IActivatorTree tree);
        bool Test(IActivatorTree tree);

        ExportMode Mode { get; }

        Func<IActivatorTree, Type> ExportType { get; set; }
        Func<IActivatorTree, DependencyLocator> Locator { get; set; }
        void AddMode(ExportMode mode);
        string ToString();

        int Priority { get; }
        void SetPriority(int priority);

        void SetSingletonLocator();
    }
}