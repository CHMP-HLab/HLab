using System;
using System.Reflection;
using HLab.Base;
using HLab.DependencyInjection.Annotations;
using HashCode = HLab.Base.HashCode;

namespace HLab.DependencyInjection
{

    public struct ImportContext : IImportContext// : IEquatable<ImportContext>
    {
        private readonly int _hash;

        private ImportContext(Type targetType, Type importType, MemberInfo targetMemberInfo, IMethodSignature signature)
        {
            TargetMemberInfo = targetMemberInfo;
            ImportType = importType;
            TargetType = targetType;
            Signature = signature;

            _hash = HashCode.Start
                .Add(targetMemberInfo)
                .Add(importType)
                .Add(targetType)
                .Add(Signature)
                .Value;
        }

        public Type ImportType { get; }
        public Type TargetType { get; }
        public MemberInfo TargetMemberInfo { get;}
        public IMethodSignature Signature { get; }


        public static ImportContext Get(Type targetType, MemberInfo mi, IMethodSignature parametersTypes = null) 
            => new ImportContext(targetType, mi.GetReturnType(), mi, parametersTypes);


        public static ImportContext Get(Type targetType, Type importType, IMethodSignature parametersTypes = null) 
            => new ImportContext(targetType, importType, null, parametersTypes);

        public IImportContext Get(Type importType, IMethodSignature parametersTypes = null) 
            => new ImportContext(TargetType,importType,TargetMemberInfo,parametersTypes);

        public override int GetHashCode() => _hash;

        public static bool operator ==(ImportContext a, IImportContext b)
        {
            if (ReferenceEquals(a,null)) return ReferenceEquals(null, b);
            if (ReferenceEquals(b, null)) return false;

            return a._hash == b.GetHashCode() 
                   && a.ImportType == b.ImportType 
                   && a.TargetType == b.TargetType 
                   && Equals(a.TargetMemberInfo, b.TargetMemberInfo) 
                   && Equals(a.Signature, b.Signature);
        }

        public static bool operator !=(ImportContext a, IImportContext b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is IImportContext ic && this == ic;
        }
    }
}
