using System;

namespace HLab.DependencyInjection.Annotations
{
    public interface IActivator
    {
        DependencyInjector GetActivator(
            Func<IActivatorTree, DependencyLocator> getLocator, IActivatorTree tree);
    }
}
