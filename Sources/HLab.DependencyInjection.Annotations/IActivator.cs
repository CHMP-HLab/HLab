using System;

namespace HLab.DependencyInjection.Annotations
{
    public interface IActivator
    {
        DependencyInjector GetActivator(
            Func<IActivatorTree, IDependencyLocator> getLocator, IActivatorTree tree);
    }
}
