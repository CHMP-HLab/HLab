using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace HLab.Notify.PropertyChanged.PropertyHelpers.Helpers;

public class PropertyEnum<T> : IPropertyValue<T>
    where T : struct, Enum, IComparable
{
    T _value;
    public T Get() => _value;

    public bool Set(T value)
    {
        if (Equals(_value, value)) return false;

        var old = Exchange(ref _value, value);

        if (!Equals(old, value))
        {
            _holder.OnPropertyChanged(old,value);
            return true;
        }
        else return false;
    }

    readonly PropertyHolder<T> _holder;

    public PropertyEnum(PropertyHolder<T> holder)
    {
        _holder = holder;
    }

    delegate T dImpl(ref T location, T value);

    static readonly dImpl Exchange = CreateCompareExchangeImpl();

    static dImpl CreateCompareExchangeImpl()
    {
        var underlyingType = Enum.GetUnderlyingType(typeof(T));
        var dynamicMethod =
            new DynamicMethod(string.Empty, typeof(T), new[] {typeof(T).MakeByRefType(), typeof(T)});
        var ilGenerator = dynamicMethod.GetILGenerator();
        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(OpCodes.Ldarg_1);
//            ilGenerator.Emit(OpCodes.Ldarg_2);
        ilGenerator.Emit(
            OpCodes.Call,
            typeof(Interlocked).GetMethod(
                "Exchange",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new[] {underlyingType.MakeByRefType(), underlyingType /*, underlyingType*/},
                null));
        ilGenerator.Emit(OpCodes.Ret);
        return (dImpl) dynamicMethod.CreateDelegate(typeof(dImpl));
    }

    public bool Set(Func<object, T> setter)
    {
        return Set(setter(_holder.Parent));
    }
}