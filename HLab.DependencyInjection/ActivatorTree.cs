using HLab.DependencyInjection.Annotations;

namespace HLab.DependencyInjection
{
    public class ActivatorTree : IActivatorTree
    {
        private IActivatorKey _key;
#if DEBUG
        public override string ToString()
        {
            var target = Context.TargetType?.GenericReadableName()??"null";
            var t2 = Context.TargetMemberInfo?.DeclaringType?.GenericReadableName()??"none";
            var member = Context.TargetMemberInfo?.Name??"unknown";
            var import = Context.ImportType?.GenericReadableName()??"unknown";

            if (target!=t2) target = target + " : "  + t2;

            return target + "." + member + " as " + import;
        }
        public string TabbedBranch()
        {
            var s = "";
            var p = Parent;
            while (p != null)
            {
                s += '\t';
                p = p.Parent;
            }

            var target = Context.TargetType?.GenericReadableName() ?? "null";
            var t2 = Context.TargetMemberInfo?.DeclaringType?.GenericReadableName() ?? target;
            var member = Context.TargetMemberInfo?.Name ?? "unknown";
            var import = Context.ImportType?.GenericReadableName() ?? "unknown";
            if (target != t2) target = target + ":" + t2;

            var actualType = Key?.ReturnType?.Name ?? "object";

            return s + target + "." + member + " as " + import + " <- " + actualType;
        }

        public string ReadableTree()
        {
            string s = "";
            IActivatorTree node = this;
            while (node != null)
            {
                s = node.ToString() + "\n" + s;
                node = node.Parent;
            }

            return s;
        }
#endif
        public IActivatorTree Parent { get; }
        public IImportContext Context { get; set; }

        public IActivatorKey Key
        {
            get => _key;
            set
            {
                _key = value;
                #if DEBUG
                if (Key != null)
                {
                    var r = GetRecursive();
                    if (r!=null) throw new DependencyInjectionException("Recursive Injection : \n" + ReadableTree());
                }
                #endif
            }
        }

        public ActivatorTree(IActivatorTree parent, IImportContext context)
        {
            Parent = parent;
            Context = context;

        }

            public IActivatorTree GetRecursive()
        {
            var parent = Parent;
            while (parent != null)
            {
                if(Equals(parent.Key,Key)) return parent;
                parent = parent.Parent;
            }

            return null;
        }

    }
}