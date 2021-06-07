using System;
using System.Reflection;
using HLab.DependencyInjection.Annotations;

namespace HLab.DependencyInjection
{
    public class ActivatorKey : IEquatable<ActivatorKey>, IActivatorKey
    {
        private readonly int _hash;
        public ActivatorKey(Type returnType, IMethodSignature signature)
        {
            ReturnType = returnType;
            Signature = signature;
            unchecked
            {
                _hash = 17;
                _hash = _hash * 23 + (ReturnType?.GetHashCode()??0);
                _hash = _hash * 23 + (Signature?.GetHashCode()??0);
            }
        }

        public Type ReturnType { get; }
        public IMethodSignature Signature { get; }

        public bool Equals(ActivatorKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (ReturnType != other.ReturnType) return false;
            if (!Equals(Signature, other.Signature)) return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ActivatorKey)obj);
        }

        public override int GetHashCode() => _hash;

        public ConstructorInfo GetConstructor()
        {
            return ReturnType.GetConstructor(Signature?.Types??Type.EmptyTypes);
        }

        public override string ToString()
        {
            return ReturnType.ToString() + ":" + Signature;
        }
    }
}
