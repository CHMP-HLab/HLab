using System;

namespace HLab.DependencyInjection.Annotations
{
    /// <summary>
    ///   Automate importation while Locate
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field| AttributeTargets.Constructor |AttributeTargets.Parameter)]
    public class ImportAttribute : Attribute
    {
        public ImportAttribute(InjectLocation location = InjectLocation.BeforeConstructor)
        {
            Location = location;
        }

        public InjectLocation Location { get; }
        //private static FieldInfo GetBackingField(PropertyInfo pi)
        //{
        //    if (!pi.CanRead || !pi.GetGetMethod(nonPublic: true).IsDefined(typeof(CompilerGeneratedAttribute), inherit: true))
        //        return null;
        //    var backingField = pi.DeclaringType.GetField($"<{pi.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
        //    if (backingField == null)
        //        return null;
        //    if (!backingField.IsDefined(typeof(CompilerGeneratedAttribute), inherit: true))
        //        return null;
        //    return backingField;
        //}

        //public virtual DependencyInjector GetActivator(
        //    Func<IActivatorTree, DependencyLocator> getLocator, IActivatorTree tree)
        //{

        //    switch (tree.Context.TargetMemberInfo)
        //    {
        //        case FieldInfo fi:
        //            var fieldCtx = tree.Context = tree.Context.Get(fi.FieldType);
        //            var fieldActivator = getLocator(tree);
        //            return (c, a, o) =>
        //            {
        //                var value = fieldActivator(c.Get(o, fieldCtx), a);
        //                fi.SetValue(o, value);
        //            };

        //        case PropertyInfo pi:
        //            if (pi.CanWrite)
        //            {
        //                var propertyCtx = tree.Context = tree.Context.Get(pi.PropertyType);
        //                var propertyActivator = getLocator(tree);
        //                return (c, a, o) =>
        //                {
        //                    var propertyInfo = pi;
        //                    var value = propertyActivator(c.Get(o, propertyCtx), a);
        //                    propertyInfo.SetValue(o, value);
        //                };
        //            }
        //            else
        //            {
        //                var fi = GetBackingField(pi);
        //                if(fi==null)
        //                    throw new Exception("Property " + pi.Name + " in " + tree.Context.TargetType.Name + " not writable");


        //                var autoCtx = tree.Context = tree.Context.Get(fi.FieldType);
        //                var autoActivator = getLocator(tree);
        //                return (c, a, o) =>
        //                {
        //                    var value = autoActivator(c.Get(o, autoCtx), a);
        //                    fi.SetValue(o, value);
        //                };
        //            }
        //        case MethodInfo mi:
        //        {
        //            Action<IRuntimeImportContext, object[], object[]> setParameters = (c, a, o) => { };
        //            var i = 0;
        //            foreach (var parameterInfo in mi.GetParameters())
        //            {
        //                var methodCtx = tree.Context = tree.Context.Get(parameterInfo.ParameterType);
        //                var methodActivator = getLocator(tree);
        //                var pos = i;
        //                setParameters += (c, a, o) => o[pos] = methodActivator(c.Get(o, methodCtx), a);
        //                i++;
        //            }

        //            var nb = i;

        //            return (c, a, o) =>
        //            {
        //                var param = new object[nb];
        //                setParameters(c, a, param);
        //                mi.Invoke(o, param);
        //            };
        //        }
        //        case ConstructorInfo ci:
        //        {
        //            Action<IRuntimeImportContext, object[], object[]> setParameters = (c, a, o) => { };
        //            var i = 0;
        //            foreach (var parameterInfo in ci.GetParameters())
        //            {
        //                var methodCtx = tree.Context = tree.Context.Get(parameterInfo.ParameterType);
        //                var methodActivator = getLocator(tree);
        //                var pos = i;
        //                setParameters += (c, a, o) => o[pos] = methodActivator(c.Get(o, methodCtx), a);
        //                i++;
        //            }

        //            var nb = i;

        //            return (c, a, o) =>
        //            {
        //                var param = new object[nb];
        //                setParameters(c, a, param);
        //                ci.Invoke(o, param);
        //            };
        //        }
        //        default:
        //            throw new Exception("Unable to import in " + tree.Context.TargetMemberInfo.Name);
        //    }
        //}
    }
}