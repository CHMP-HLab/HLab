using System;

namespace HLab.Notify.PropertyChanged;

public interface IPropertyValue
{
}
public interface IPropertyValue<T>: IPropertyValue
{
    bool Set(T value);
    T Get();
    bool Set(Func<object, T> value);

}