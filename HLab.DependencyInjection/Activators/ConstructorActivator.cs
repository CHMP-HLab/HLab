﻿using System;
using System.Reflection;
using HLab.DependencyInjection.Annotations;

namespace HLab.DependencyInjection.Activators
{
    public class ConstructorActivator : IActivator
    {
        public DependencyInjector GetActivator(Func<IActivatorTree, DependencyLocator> getLocatorFunc, IActivatorTree tree)
        {
            if (tree.Context.TargetMemberInfo is ConstructorInfo ci)
            {
                Action<IRuntimeImportContext, object[], object[]> setParameters = null;
                var i = 0;
                foreach (var parameterInfo in ci.GetParameters())
                {
                    var ctx = tree.Context = tree.Context.Get(parameterInfo.ParameterType);
                    var locator = getLocatorFunc(tree);
                    var pos = i;
                    setParameters += (c, a, o) => o[pos] = locator(c.Get(o, ctx), a);
                    i++;
                }

                var nb = i;

                if(setParameters!=null)
                    return (c, a, o) =>
                    {
                        var param = new object[nb];
                        setParameters.Invoke(c, a, param);
                        ci.Invoke(o, param);
                    };
                return (c, a, o) => ci.Invoke(o, new object[0]);
            }

            throw new InvalidOperationException(tree.Context.TargetMemberInfo.Name + "is not constructor");
        }
    }
}