using System;
using System.Collections.Specialized;

namespace HLab.Notify.PropertyChanged;

public static class FilterConfiguratorExtension
{
    public static NotifyConfigurator<TClass, ObservableFilterPropertyHolder<T>> AddFilter<TClass, T>(this NotifyConfigurator<TClass, ObservableFilterPropertyHolder<T>> c,
        Func<T, bool> expression)
        where TClass : class, INotifyPropertyChangedWithHelper
    {
        return c
            .Do((target,filter)=>filter.AddFilter(expression));
    }
    public static NotifyConfigurator<TClass, ObservableFilterPropertyHolder<T>> AddFilter<TClass, T>(this NotifyConfigurator<TClass, ObservableFilterPropertyHolder<T>> c,
        Func<TClass,T, bool> expression)
        where TClass : class, INotifyPropertyChangedWithHelper
    {
        return c
            .Do((target,filter)=>filter.AddFilter(e => expression(target,e)));
    }

    public static NotifyConfigurator<TClass, TFilter> Link<TClass, TFilter>(this NotifyConfigurator<TClass, TFilter> c,
        Func<TClass,INotifyCollectionChanged> expression)
        where TClass : class, INotifyPropertyChangedWithHelper
        where TFilter : class, IObservableFilter
    {
        return c
            .Do((target,filter)=>filter.Link(() => expression(target)));
    }

    //public static NotifyConfigurator<TClass, TMember> 
    //    Update<TClass,TMember>(this NotifyConfigurator<TClass, TMember> c)
    //    where TClass : class, INotifyPropertyChanged
    //    where TMember : IObservableFilter
    //{
    //    return c.Do((target, member) => member.OnTriggered());
    //}
}