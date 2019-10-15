using System.Diagnostics;
using System.Reflection;

namespace HLab.Notify
{
    public class NotifierPropertyReflexion<TClass,T> : NotifierProperty<TClass,T>, INotifierPropertyReflexion<T>
    {
        public PropertyInfo PropertyInfo { get; }

        public NotifierPropertyReflexion(NotifierClass<TClass> cls, PropertyInfo pi):base(cls,pi.Name)
        {
            Debug.Assert(pi.PropertyType==typeof(T));

            PropertyInfo = pi;
        }
    }
}