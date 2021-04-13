using System;
using System.Reflection;

namespace HLab.DependencyInjection.Annotations
{
    public interface IImportContext
    {
        Type ImportType { get; }
        IMethodSignature Signature { get; }
        MemberInfo TargetMemberInfo { get; }

        /// <summary>
        /// Type 
        /// </summary>
        Type TargetType { get; }

        bool Equals(object obj);
        IImportContext Get(Type importType, IMethodSignature parametersTypes = null);
        IImportContext<T> Get<T>(IMethodSignature parametersTypes = null);
        int GetHashCode();
    }

    public interface IImportContext<T> : IImportContext
    {

    }
}