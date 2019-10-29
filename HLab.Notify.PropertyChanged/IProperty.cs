using System.Runtime.CompilerServices;

namespace HLab.Notify.PropertyChanged
{
    public interface IForeign<T> : IChildObject
    {
        IProperty<int?> Id { get; }
        //IProperty<T> Value{ get; }

#if DEBUG
        T Get([CallerMemberName]string name = null);
#else        
        T Get() => Value.Get();
#endif
        void Set(T value);
    }


    public interface IProperty<T> : IProperty, IChildObject
    {
        bool Set(T value);


#if DEBUG
        T Get([CallerMemberName]string name=null);
#else
        T Get();
#endif
    }

    public interface IProperty
    {
    }
}