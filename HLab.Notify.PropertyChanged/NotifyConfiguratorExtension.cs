using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;
using HLab.Notify.Annotations;

namespace HLab.Notify.PropertyChanged
{
    public static class NotifyConfiguratorExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static NotifyConfigurator<TClass, T> On<TClass,T>(this NotifyConfigurator<TClass, T> c, Expression<Func<TClass, object>> expr)
            where TClass : class//,INotifyPropertyChanged
        {
            return c.TriggerExpression(expr);
        }


        public static NotifyConfigurator<TClass, T> NotNull<TObj, TClass, T>(this NotifyConfigurator<TClass, T> c, Func<TClass, TObj> notnull)
            where TClass : class//, INotifyPropertyChanged
            //            where TObj : class
        {
            return c.When(e => ! Equals(notnull(e), default(TObj)));
        }

    }
}