using System;
using System.Reflection;

namespace HLab.DependencyInjection.Annotations
{
    public interface IImportContext
    {
        Type ImportType { get; }
        IMethodSignature Signature { get; }
        MemberInfo TargetMemberInfo { get; }
        Type TargetType { get; }

        bool Equals(object obj);
        IImportContext Get(Type importType, IMethodSignature parametersTypes = null);
        int GetHashCode();
    }
}