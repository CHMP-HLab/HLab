using System;

namespace HLab.DependencyInjection.Annotations
{
    public delegate void DependencyInjector(IRuntimeImportContext ctx, object[] args, object target);
    //public delegate T DependencyLocator<out T>(IRuntimeImportContext ctx, object[] args);
    //public delegate object DependencyLocator(IRuntimeImportContext ctx, object[] args);

    public interface IDependencyLocator
    {
        public object Locate(IRuntimeImportContext ctx, object[] args);
    }
    public interface IDependencyLocator<out T> : IDependencyLocator
    {
        public new T Locate(IRuntimeImportContext ctx, object[] args);
    }

    public class DependencyLocator<T> : IDependencyLocator<T>
    {
        private readonly Func<IRuntimeImportContext,object[],T> _locator;
        public DependencyLocator(Func<IRuntimeImportContext,object[],T> locator)
        {
            _locator = locator;
        }

        public T Locate(IRuntimeImportContext ctx, object[] args) => _locator(ctx,args);

        object IDependencyLocator.Locate(IRuntimeImportContext ctx, object[] args) => _locator(ctx,args);

        public static implicit operator DependencyLocator<T>(Func<IRuntimeImportContext, object[], T> locator) =>
            new(locator);
    }


    public struct DependencyInjectorSet
    {
        public DependencyInjector PreConstructor;
        public DependencyInjector Constructor;
        public DependencyInjector PostConstructor;
    }

    public interface IExportEntry
    {
        IExportLocatorScope Scope { get; }
        void AndCondition(Func<IActivatorTree, bool> condition);
        void OrCondition(Func<IActivatorTree, bool> condition);
        Func<IRuntimeImportContext, bool> RuntimeCondition { get; set; }

        IActivatorKey GetActivatorKey(IActivatorTree tree);
        bool Test(IActivatorTree tree);

        ExportMode Mode { get; }

        Func<IActivatorTree, Type> ExportType { get; set; }
        Func<IActivatorTree, IDependencyLocator> Locator { get; set; }

        void AddMode(ExportMode mode);
        string ToString();

        int Priority { get; }
        void SetPriority(int priority);

        void SetSingletonLocator();
    }
}