using System;
using System.Threading.Tasks;

namespace HLab.Notify.PropertyChanged;

public static class CommandConfiguratorExtension
{
    public static NotifyConfigurator<TClass, T> CanExecute<TClass, T>(this NotifyConfigurator<TClass, T> c,
        Func<TClass, bool> setter)
        where TClass : class, INotifyPropertyChangedWithHelper
        where T : CommandPropertyHolder
    {
        return c
            .Do((target,cmd)=>cmd.SetCanExecute(() => setter(target)));
    }
    public static NotifyConfigurator<TClass, T> CanExecute<TClass, T>(this NotifyConfigurator<TClass, T> c,
        Func<TClass,object, bool> setter)
        where TClass : class, INotifyPropertyChangedWithHelper
        where T : CommandPropertyHolder
    {
        return c
            .Do((target,cmd)=>cmd.SetCanExecute(o => setter(target,o)));
    }

    public static NotifyConfigurator<TClass, T> Action<TClass, T>(this NotifyConfigurator<TClass, T> c,
        Action<TClass> action)
        where TClass : class, INotifyPropertyChangedWithHelper
        where T : CommandPropertyHolder
    {
        return c
            .On().Do((target,cmd)=>cmd.SetAction(() => action(target)));
    }

    public static NotifyConfigurator<TClass, T> Action<TClass, T>(this NotifyConfigurator<TClass, T> c,
        Func<TClass,Task> action)
        where TClass : class, INotifyPropertyChangedWithHelper
        where T : CommandPropertyHolder
    {
        return c
            .On().Do((target,cmd)=>cmd.SetAction( () => action(target)));
    }
    public static NotifyConfigurator<TClass, T> Action<TClass, T>(this NotifyConfigurator<TClass, T> c,
        Action<TClass,object> action)
        where TClass : class, INotifyPropertyChangedWithHelper
        where T : CommandPropertyHolder
    {
        return c
            .Do((target,cmd)=>cmd.SetAction(o => action(target,o)));
    }
    public static NotifyConfigurator<TClass, T> Action<TClass, T>(this NotifyConfigurator<TClass, T> c,
        Func<TClass,object,Task> actionAsync)
        where TClass : class, INotifyPropertyChangedWithHelper
        where T : CommandPropertyHolder
    {
        return c
            .Do((target,cmd)=>cmd.SetAction( o => actionAsync(target,o)));
    }

    public static NotifyConfigurator<TClass, TMember> 
        CheckCanExecute<TClass,TMember>(this NotifyConfigurator<TClass, TMember> c)
        where TClass : class, INotifyPropertyChangedWithHelper
        where TMember : CommandPropertyHolder
    {
        return c.Do((target, member) => member.SetCanExecute());
    }
    public static NotifyConfigurator<TClass, TMember> 
        CheckCanExecute<TClass,TMember>(this NotifyConfigurator<TClass, TMember> c, Func<TClass,object> getter)
        where TClass : class, INotifyPropertyChangedWithHelper
        where TMember : CommandPropertyHolder
    {
        return c.Do((target, member) => member.SetCanExecute(getter(target)));
    }
    public static NotifyConfigurator<TClass, TMember> 
        CheckCanExecute<TClass,TMember>(this NotifyConfigurator<TClass, TMember> c, Func<TClass,bool> getter)
        where TClass : class, INotifyPropertyChangedWithHelper
        where TMember : CommandPropertyHolder
    {
        return c.Do((target, member) => member.SetCanExecute(getter(target)));
    }

    public static NotifyConfigurator<TClass, TMember> 
        IconPath<TClass,TMember>(this NotifyConfigurator<TClass, TMember> c, string iconPath)
        where TClass : class, INotifyPropertyChangedWithHelper
        where TMember : CommandPropertyHolder
    {
        return c.Do((target, member) => member.IconPath = iconPath);
    }

    public static NotifyConfigurator<TClass, TMember> 
        ToolTip<TClass,TMember>(this NotifyConfigurator<TClass, TMember> c, string toolTip)
        where TClass : class, INotifyPropertyChangedWithHelper
        where TMember : CommandPropertyHolder
    {
        return c.Do((target, member) => member.IconPath = toolTip);
    }
}