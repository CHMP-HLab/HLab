using System;
using System.Reflection;
using HLab.DependencyInjection.Annotations;

namespace HLab.DependencyInjection
{

    public readonly struct ImportContext : IImportContext// : IEquatable<ImportContext>
    {
        private readonly int _hash;

        internal ImportContext(Type targetType, Type importType, MemberInfo targetMemberInfo, IMethodSignature signature)
        {
            TargetMemberInfo = targetMemberInfo;
            ImportType = importType;
            TargetType = targetType;
            Signature = signature;

            _hash = HashCode.Combine(targetMemberInfo,importType,targetType,Signature);
        }

        public Type ImportType { get; }
        public Type TargetType { get; }
        public MemberInfo TargetMemberInfo { get;}
        public IMethodSignature Signature { get; }

        public int ParamIndex => throw new NotImplementedException();

        public static IImportContext Create(Type targetType, MemberInfo mi, IMethodSignature parametersTypes = null) 
            => new ImportContext(targetType, mi.GetReturnType(), mi, parametersTypes);

        public static IImportContext Create(Type targetType, Type importType, IMethodSignature parametersTypes = null) 
            => new ImportContext(targetType, importType, null, parametersTypes);

        public IImportContext CreateChild(Type importType, IMethodSignature parametersTypes = null) 
            => new ImportContext(TargetType,importType,TargetMemberInfo,parametersTypes);

        public IImportContext<T> CreateChild<T>(IMethodSignature parametersTypes = null)
            => new ImportContext<T>(TargetType,TargetMemberInfo,parametersTypes);

        public override int GetHashCode() => _hash;

        public static bool operator ==(ImportContext a, IImportContext b)
        {
            if (b is null) return false;

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

        public override string ToString()
        {
            return $"{ImportType?.Name} in {TargetType?.Name} = {TargetMemberInfo?.DeclaringType?.Name}{TargetMemberInfo?.Name}{Signature}";
        }
    }
    public readonly struct ImportContext<T> : IImportContext<T>// : IEquatable<ImportContext>
    {
        private readonly int _hash;

        internal ImportContext(Type targetType, MemberInfo targetMemberInfo, IMethodSignature signature)
        {
            TargetMemberInfo = targetMemberInfo;
            TargetType = targetType;
            Signature = signature;

            _hash = HashCode.Combine(targetMemberInfo,typeof(T),targetType,Signature);
        }

        public Type ImportType => typeof(T);
        public Type TargetType { get; }
        public MemberInfo TargetMemberInfo { get;}
        public IMethodSignature Signature { get; }

        public int ParamIndex => throw new NotImplementedException();

        public static IImportContext Create(Type targetType, MemberInfo mi, IMethodSignature parametersTypes = null) 
            => new ImportContext(targetType, mi.GetReturnType(), mi, parametersTypes);


        public static IImportContext Create(Type targetType, Type importType, IMethodSignature parametersTypes = null) 
            => new ImportContext(targetType, importType, null, parametersTypes);

        public IImportContext CreateChild(Type importType, IMethodSignature parametersTypes = null) 
            => new ImportContext(TargetType,importType,TargetMemberInfo,parametersTypes);

        public IImportContext<T1> CreateChild<T1>(IMethodSignature parametersTypes = null)
            => new ImportContext<T1>(TargetType,TargetMemberInfo,parametersTypes);

        public override int GetHashCode() => _hash;

        public static bool operator ==(ImportContext<T> a, IImportContext b)
        {
            if (b is null) return false;

            return a._hash == b.GetHashCode() 
                   && a.ImportType == b.ImportType 
                   && a.TargetType == b.TargetType 
                   && Equals(a.TargetMemberInfo, b.TargetMemberInfo) 
                   && Equals(a.Signature, b.Signature);
        }

        public static bool operator !=(ImportContext<T> a, IImportContext b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is IImportContext ic && this == ic;
        }
        public override string ToString()
        {
            return $"{ImportType.Name} as {TargetType.Name} => {TargetMemberInfo.DeclaringType?.Name}{TargetMemberInfo.Name}{Signature}";
        }
    }
}
