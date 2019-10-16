using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HLab.Notify.Annotations;

namespace HLab.Notify.PropertyChanged
{
    public static class PropertyConfiguratorExtension
    {
        /// <summary>
        /// Set property value
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <typeparam name="TMember">PropertyHolder type</typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <param name="setter"></param>
        /// <returns></returns>
        public static NotifyConfigurator<TClass, TMember> 
            Set<TClass, TMember, T>(this NotifyConfigurator<TClass, TMember> c, Func<TClass, T> setter)
            where TClass : class//, INotifyPropertyChanged
            where TMember : PropertyHolder<T>
        {

            if (c.CurrentTrigger.TriggerOnList.Count == 0)
            {
                return c.On().Do((target, property) => property.SetProperty(new PropertyValueLazy<T>(property, o => setter((TClass)o))));
            }

            return c.Do((target, property) => { property.PropertyValue.Set(setter(target)); });

        }

        public static NotifyConfigurator<TClass, TMember> 
            Set<TClass, TMember, T>(this NotifyConfigurator<TClass, TMember> c, Func<TClass, Task<T>> setter)
            where TClass : class//, INotifyPropertyChanged
            where TMember : PropertyHolder<T>
        {

            if (c.CurrentTrigger.TriggerOnList.Count == 0)
            {
                return c.On().Do((target, property) => property.SetProperty(new PropertyValueLazy<T>(property, o =>
                {
                    var t = setter((TClass) o);
                    t.Wait();
                    return t.Result;
                })));
            }

            return c.Do(async (target, property) =>
            {
                property.PropertyValue.Set(await setter(target));
            });

        }
        /// <summary>
        /// Alter property value
        /// </summary>
        /// <typeparam name="TClass">Target class</typeparam>
        /// <typeparam name="TMember">PropertyHolder type</typeparam>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="c"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static NotifyConfigurator<TClass, TMember> 
            Set<TClass, TMember,T>(this NotifyConfigurator<TClass, TMember> c, Action<TClass, T> action)
            where TClass : class, INotifyPropertyChanged
            where TMember : PropertyHolder<T>
        {
            return c.Do((target, property) => action(target, property.Get()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TClass">Target class</typeparam>
        /// <typeparam name="TMember">PropertyHolder type</typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        public static NotifyConfigurator<TClass, TMember> 
            Trig<TClass,TMember,T>(this NotifyConfigurator<TClass, TMember> c)
            where T : ITriggerable
            where TClass : class, INotifyPropertyChanged
            where TMember : PropertyHolder<T>
        {
            return c.Do((target, property) => property.Get().OnTriggered());
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TClass">Target class</typeparam>
        /// <typeparam name="TMember">PropertyHolder type</typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <param name="default"></param>
        /// <returns></returns>
        public static NotifyConfigurator<TClass, TMember> 
            Default<TClass, TMember, T>(this NotifyConfigurator<TClass, TMember> c, T @default)
            where TClass : class//, INotifyPropertyChanged
            where TMember : PropertyHolder<T>
        {
            return c.Set(target => @default);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <typeparam name="TMember"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static NotifyConfigurator<TClass, TMember> 
            OneWayBind<TClass, TMember,T>(this NotifyConfigurator<TClass, TMember> c, Expression<Func<TClass, T>> expr)
            where TClass : class, INotifyPropertyChanged
            where TMember : PropertyHolder<T>
        {
            return c.TriggerExpression(expr).Set(expr.Compile());
        }

    }
}