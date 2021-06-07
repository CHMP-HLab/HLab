using System;
using System.Reflection;

namespace HLab.DependencyInjection.Annotations
{
    public interface IImportContext
    {
        Type ImportType { get; }
        IMethodSignature Signature { get; }
        MemberInfo TargetMemberInfo { get; }
        int ParamIndex { get; }
        /// <summary>
        /// Type 
        /// </summary>
        Type TargetType { get; }

        bool Equals(object obj);
        IImportContext CreateChild(Type importType, IMethodSignature signature = null);
        IImportContext<T> CreateChild<T>(IMethodSignature signature = null);
        int GetHashCode();
    }

    public interface IImportContext<T> : IImportContext
    {

    }
}