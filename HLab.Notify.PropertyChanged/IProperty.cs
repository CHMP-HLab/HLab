using System.Runtime.CompilerServices;

namespace HLab.Notify.PropertyChanged;

public interface IForeign<T> : IChildObject
{
    IProperty<int?> Id { get; }
    //IProperty<T> Value{ get; }

#if DEBUG
    T Get([CallerMemberName]string name = null);
#else        
        T Get();
#endif
    void Set(T value);
}


public interface IProperty<T> : IProperty, IChildObject
{
    T Value { get; set; }
    bool Set(T value);
    void Update();
    T GetNoCheck();

#if DEBUG
    T Get([CallerMemberName]string name=null);
#else
        T Get();
#endif
}

public interface IProperty
{
}