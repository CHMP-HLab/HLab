using HLab.DependencyInjection.Annotations;

namespace HLab.DependencyInjection.UTests
{
    interface IInjectedAsA
    { }

    interface IInjectedAsB
    { }

    [Export(typeof(IInjectedAsA))]
    [Export(typeof(IInjectedAsB))]
    class InjectedClassDualInterfaces : IInjectedAsA, IInjectedAsB
    {

    }
}