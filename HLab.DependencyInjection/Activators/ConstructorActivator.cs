using System;
using System.Reflection;
using HLab.DependencyInjection.Annotations;

namespace HLab.DependencyInjection
{
    public class ConstructorActivator : IActivator
    {
        public DependencyInjector GetActivator(Func<IActivatorTree, DependencyLocator> getLocator, IActivatorTree tree)
        {
            if (tree.Context.TargetMemberInfo is ConstructorInfo ci)
            {
                Action<IRuntimeImportContext, object[], object[]> setParameters = (c, a, o) => { };
                var i = 0;
                foreach (var parameterInfo in ci.GetParameters())
                {
                    var methodCtx = tree.Context = tree.Context.Get(parameterInfo.ParameterType);
                    var methodActivator = getLocator(tree);
                    var pos = i;
                    setParameters += (c, a, o) => o[pos] = methodActivator(c.Get(o, methodCtx), a);
                    i++;
                }

                var nb = i;

                return (c, a, o) =>
                {
                    var param = new object[nb];
                    setParameters(c, a, param);
                    ci.Invoke(o, param);
                };
            }
            else throw new InvalidOperationException(tree.Context.TargetMemberInfo.Name + "is not constructor");
        }
    }
}