using System;
using System.ComponentModel;

namespace HLab.Notify.PropertyChanged
{
    public static class CommandConfiguratorExtension
    {
        public static NotifyConfigurator<TClass, T> CanExecute<TClass, T>(this NotifyConfigurator<TClass, T> c,
            Func<TClass, bool> setter)
            where TClass : class, INotifyPropertyChanged
            where T : NotifyCommand
        {
            return c
                .Do((target,cmd)=>cmd.SetCanExecute(() => setter(target)));
        }
        public static NotifyConfigurator<TClass, T> SetCanExecute<TClass, T>(this NotifyConfigurator<TClass, T> c,
            Func<TClass,object, bool> setter)
            where TClass : class//, INotifyPropertyChanged
            where T : NotifyCommand
        {
            return c
                .Do((target,cmd)=>cmd.SetCanExecute(o => setter(target,o)));
        }

        public static NotifyConfigurator<TClass, T> Action<TClass, T>(this NotifyConfigurator<TClass, T> c,
            Action<TClass> action)
            where TClass : class//, INotifyPropertyChanged
            where T : NotifyCommand
        {
            return c
                .On().Do((target,cmd)=>cmd.SetAction(() => action(target)));
        }
        public static NotifyConfigurator<TClass, T> Action<TClass, T>(this NotifyConfigurator<TClass, T> c,
            Action<TClass,object> action)
            where TClass : class//, INotifyPropertyChanged
            where T : NotifyCommand
        {
            return c
                .Do((target,cmd)=>cmd.SetAction(o => action(target,o)));
        }

        public static NotifyConfigurator<TClass, TMember> 
            CheckCanExecute<TClass,TMember>(this NotifyConfigurator<TClass, TMember> c)
            where TClass : class, INotifyPropertyChanged
            where TMember : NotifyCommand
        {
            return c.Do((target, member) => member.SetCanExecute());
        }

    }
}