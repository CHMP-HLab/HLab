using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HLab.DependencyInjection.Annotations;

namespace HLab.DependencyInjection
{
    public class MethodActivator : IActivator
    {
        public DependencyInjector GetActivator(Func<IActivatorTree, IDependencyLocator> getLocator, IActivatorTree tree)
        {
            if (tree.Context.TargetMemberInfo is MethodInfo mi)
            {
                Action<IRuntimeImportContext, object[], object[]> setParameters = (c, a, o) => { };
                var i = 0;
                foreach (var parameterInfo in mi.GetParameters())
                {
                    var methodCtx = tree.Context = tree.Context.CreateChild(parameterInfo.ParameterType);
                    var methodActivator = getLocator(tree);
                    var pos = i;
                    setParameters += (c, a, o) => o[pos] = methodActivator.Locate(c.NewChild(o, methodCtx), a);
                    i++;
                }

                var nb = i;

                return (c, a, o) =>
                {
                    var param = new object[nb];
                    setParameters(c, a, param);
                    mi.Invoke(o, param);
                };
            }
            else throw new InvalidOperationException(tree.Context.TargetMemberInfo.Name + "is not method");
        }
    }
}
