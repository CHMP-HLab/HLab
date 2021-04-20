using System;
using System.Linq;
using HLab.DependencyInjection.Annotations;
using HashCode = HLab.Base.HashCode;

namespace HLab.DependencyInjection
{
    public class MethodSignature : IEquatable<IMethodSignature>, IMethodSignature
    {
        public Type[] Types { get; }
        private readonly int _hash;

        public MethodSignature(params Type[] parameters)
        {
            Types = parameters;
            _hash = _hash = HashCode.Start
                .Add(parameters)
                .Value;

            foreach (var type in parameters)
            {
                _hash = 31 * type.GetHashCode();
            }
        }

        public MethodSignature(params object[] arguments) : this(arguments.GetTypes())
        {
        }

        public override int GetHashCode()
        {
            return _hash;
        }


        public bool Equals(IMethodSignature other)
        {
            if (Types == null) return other?.Types == null;

            if (other?.Types == null) return false;

            if (Types.Length != other.Types.Length) return false;

            for (var i = 0; i < Types.Length; i++)
            {
                if (Types[i] != other.Types[i]) return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MethodSignature) obj);
        }

        public override string ToString()
        {
            if (Types == null) return "(null)";
            return "(" + String.Join(",",Types.Select(t => t.Name)) + ")";
        }
    }
}
