using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HLab.Notify.PropertyChanged.PropertyHelpers;

namespace HLab.Notify.PropertyChanged;

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
        where TClass : class, INotifyPropertyChangedWithHelper
        where TMember : class, IProperty<T>
    {
        if (c.CurrentTrigger.TriggerOnList.Count == 0)
        {
            var action = c.GetDoWhenAction((target, property) => property.Set(setter(target)));
            c.Init(action);
            return c.On().Do((target, property) =>
            {
                //property.Set(setter((TClass)target));
            });
        }

        return c.Do((target, property) => property.Set(setter(target)));
    }

    public static NotifyConfigurator<TClass, TMember> 
        Set<TClass, TMember, T>(this NotifyConfigurator<TClass, TMember> c, Func<TClass, Task<T>> setter)
        where TClass : class, INotifyPropertyChangedWithHelper
        where TMember : PropertyHolder<T>
    {

        if (c.CurrentTrigger.TriggerOnList.Count == 0)
        {
            return c.On().Do((target, property) => property.SetProperty(new PropertyValueLazy<T>(property, o =>
            {
                var t = setter((TClass) o);
                if (t.Wait(5000))
                {
                    return t.Result;
                }
                return default(T);
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
        where TClass : class, INotifyPropertyChangedWithHelper
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
        where TClass : class, INotifyPropertyChangedWithHelper
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
        where TClass : class, INotifyPropertyChangedWithHelper
        where TMember : class,IProperty<T>
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
        Bind<TClass, TMember,T>(this NotifyConfigurator<TClass, TMember> c, Expression<Func<TClass, T>> expr)
        where TClass : class, INotifyPropertyChangedWithHelper
        where TMember : class, IProperty<T>
    {
        return c.Set(expr.Compile()).AddTriggerExpression(expr).Update();
    }
    //public static PropertyHolder<TMember> Bind<TClass,TMember,T>(this NotifyHelper<TClass> helper, Expression<Func<TClass, T>> expr) 
    //    where TClass : class, INotifyPropertyChanged
    //    => NotifyHelper<TClass>.Property<TMember>(c => c.OneWayBind(expr));

}