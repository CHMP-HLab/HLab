using System;
using System.Reflection;
using HLab.DependencyInjection.Annotations;

namespace HLab.DependencyInjection.Activators
{
    public class FieldActivator : IActivator
    {
        public DependencyInjector GetActivator(Func<IActivatorTree, IDependencyLocator> getLocator, IActivatorTree tree)
        {
            if (tree.Context.TargetMemberInfo is FieldInfo fi)
            {
                var fieldCtx = tree.Context = tree.Context.CreateChild(fi.FieldType);
                var fieldActivator = getLocator(tree);
                return (c, a, o) =>
                {
                    var value = fieldActivator.Locate(c.NewChild(o, fieldCtx), a);
                    fi.SetValue(o, value);
                };
            }
            else throw new InvalidOperationException(tree.Context.TargetMemberInfo.Name + "is not field");
        }
    }
}