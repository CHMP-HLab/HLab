using System;
using System.Reflection;
using HLab.DependencyInjection.Annotations;
using HLab.DependencyInjection;
using HLab.Notify.Annotations;

namespace HLab.Notify
{
    public class NotifierPropertyActivator : PropertyActivator
    {
        private static readonly MethodInfo GetPropertyEntryInfo = typeof(INotifier).GetMethod("GetPropertyEntry",new []{typeof(PropertyInfo)});
        private static readonly MethodInfo SetInitialValueInfo = typeof(INotifierPropertyEntry).GetMethod("SetInitialValue");

        public override DependencyInjector GetActivator(Func<IActivatorTree, DependencyLocator> getLocator, IActivatorTree tree)
        {
            if (tree.Context.TargetMemberInfo is PropertyInfo pi)
            {
                if (pi.CanWrite) return base.GetActivator(getLocator, tree);

                var backingField = GetBackingField(pi);

                var propertyCtx = tree.Context.Get(pi.PropertyType,null);
                var propertyActivator = getLocator(new ActivatorTree(tree,propertyCtx));

                var getPropertyEntry = GetPropertyEntryInfo.MakeGenericMethod(pi.PropertyType);


                if (backingField != null)
                {
                    return (c, args, target) =>
                    {
                        var value = propertyActivator(c.Get(target, propertyCtx), args);
                        backingField.SetValue(target, value);

                        var notifier = (target as INotifierObject)?.GetNotifier();
                        if (notifier != null)
                        {
                            var prm = new object[] {pi};
                            var entry = getPropertyEntry.Invoke(notifier,prm);

                            SetInitialValueInfo.Invoke(entry, new[] {value});
                            return;
                        }
                        throw new Exception("No notifier available ");
                    };
                }
                else
                {
                    return (c, args, target) =>
                    {
                        var notifier = (target as INotifierObject)?.GetNotifier();
                        if (notifier != null)
                        {
                            var prm = new object[] {pi};
                            var entry = getPropertyEntry.Invoke(notifier,prm);

                            var value = propertyActivator(c.Get(target, propertyCtx), args);
                            SetInitialValueInfo.Invoke(entry, new[] {value});
                            return;
                        }
                        throw new Exception("No notifier available ");
                    };
                    
                }
                }

            throw new Exception(tree.Context.TargetMemberInfo.Name + " is not a property");
        }
    }
}
