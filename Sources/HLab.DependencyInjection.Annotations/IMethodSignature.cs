using System;

namespace HLab.DependencyInjection.Annotations
{
    public interface IMethodSignature
    {
        Type[] Types { get; }

        bool Equals(IMethodSignature other);
        bool Equals(object obj);
        int GetHashCode();
    }
}