using System;
using System.Reflection;

namespace HLab.DependencyInjection.Annotations
{
    public interface IActivatorKey
    {
        Type ReturnType { get; }
        IMethodSignature Signature { get; }
        ConstructorInfo GetConstructor();
    }
}