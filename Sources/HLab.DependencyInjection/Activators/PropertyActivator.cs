using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using HLab.DependencyInjection.Annotations;

namespace HLab.DependencyInjection
{
    public class PropertyActivator : IActivator
    {
        protected static FieldInfo GetBackingField(PropertyInfo pi)
        {
            if (!pi.CanRead || !pi.GetGetMethod(nonPublic: true).IsDefined(typeof(CompilerGeneratedAttribute), inherit: true))
                return null;
            var backingField = pi.DeclaringType.GetField($"<{pi.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            if (backingField == null)
                return null;
            if (!backingField.IsDefined(typeof(CompilerGeneratedAttribute), inherit: true))
                return null;
            return backingField;
        }
        public virtual DependencyInjector GetActivator(Func<IActivatorTree, IDependencyLocator> getLocator, IActivatorTree tree)
        {
            if (tree.Context.TargetMemberInfo is PropertyInfo pi)
            {
                if (pi.CanWrite)
                {
                    var propertyCtx = tree.Context = tree.Context.CreateChild(pi.PropertyType);
                    var propertyActivator = getLocator(tree);
                    return (c, args, o) =>
                    {
                        var propertyInfo = pi;
                        var value = propertyActivator.Locate(c.NewChild(o, propertyCtx), args);
                        propertyInfo.SetValue(o, value);
                    };
                }
                else
                {
                    var fi = GetBackingField(pi);
                    if (fi == null)
                        throw new Exception("Property " + pi.Name + " in " + tree.Context.TargetType.Name + " not writable");


                    var autoCtx = tree.Context = tree.Context.CreateChild(fi.FieldType);
                    var autoActivator = getLocator(tree);
                    return (c, args, target) =>
                    {
                        var value = autoActivator.Locate(c.NewChild(target, autoCtx), args);
                        fi.SetValue(target, value);
                    };
                }
            }
            else throw new InvalidOperationException(tree.Context.TargetMemberInfo.Name + "is not property");
        }
    }
}