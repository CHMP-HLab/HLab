using System;

namespace HLab.DependencyInjection.Annotations
{
    public delegate void DependencyInjector(object[] args, object target);
    //public delegate T DependencyLocator<out T>(IRuntimeImportContext ctx, object[] args);
    //public delegate object DependencyLocator(IRuntimeImportContext ctx, object[] args);

    public interface IDependencyLocator
    {
        public object Locate(object[] args);
    }
    public interface IDependencyLocator<out T> : IDependencyLocator
    {
        public new T Locate(object[] args);
    }

    public class DependencyLocator<T> : IDependencyLocator<T>
    {
        private readonly Func<object[],T> _locator;
        public DependencyLocator(Func<object[],T> locator)
        {
            _locator = locator;
        }

        public T Locate(object[] args) => _locator(args);

        object IDependencyLocator.Locate(object[] args) => _locator(args);

        public static implicit operator DependencyLocator<T>(Func<object[], T> locator) =>
            new(locator);
    }


    public struct DependencyInjectorSet
    {
        public DependencyInjector Constructor;
        public DependencyInjector PostConstructor;
    }

    public interface IExportEntry
    {
        IExportLocatorScope Scope { get; }
        void AndCondition(Func<IActivatorTree, bool> condition);
        void OrCondition(Func<IActivatorTree, bool> condition);

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