using System;
using System.Linq.Expressions;

namespace HLab.Notify.PropertyChanged
{
    public static class NotifyConfiguratorExtension
    {
        public static NotifyConfigurator<TClass, T> 
            On<TClass,T>(this NotifyConfigurator<TClass, T> c, Expression<Func<TClass, object>> expr)
            where TClass : class//,INotifyPropertyChanged
        {
            return c.AddTriggerExpression(expr);
        }


        public static NotifyConfigurator<TClass, T> 
            NotNull<TObj, TClass, T>(this NotifyConfigurator<TClass, T> c, Func<TClass, TObj> notnull)
            where TClass : class//, INotifyPropertyChanged
            //            where TObj : class
        {
            return c.When(e => ! Equals(notnull(e), default(TObj)));
        }

        public static NotifyConfigurator<TClass, T> 
            OnNotNull<TObj, TClass,T>(this NotifyConfigurator<TClass, T> c, Expression<Func<TClass, TObj>> expr)
            where TClass : class//,INotifyPropertyChanged
        {
            var f = expr.Compile();
            return c.AddTriggerExpression(expr).NotNull(f);
        }
    }
}