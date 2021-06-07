using HLab.Notify.Annotations;
using System.Reflection;

namespace HLab.Notify
{
    public interface INotifierPropertyReflexion<T> : INotifierProperty<T>
    {
        PropertyInfo PropertyInfo { get; }
    }


}
