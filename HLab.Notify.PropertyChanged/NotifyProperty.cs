using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HLab.Notify.PropertyChanged;

public abstract class NotifyPropertyA
{
    public class NotifyPropertyValue
    {

    }

}

public class NotifyPropertyA<TClass, TValue> : NotifyPropertyA
{
    public new class NotifyPropertyValue : NotifyPropertyA.NotifyPropertyValue
    {
        TValue _value;
        public TValue Get() => _value;
        public bool Set(TValue value)
        {
            if (Equals(_value, value)) return false;
            _value = value;
            return true;
        }
    }

    NotifyPropertyA(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public static NotifyPropertyA<TClass, TValue> Register(string name) => new NotifyPropertyA<TClass, TValue>(name);

    public TValue Get(NotifyClass<TClass>.Notifier n) => n.Get(this);
    public void Set(NotifyClass<TClass>.Notifier n, TValue value) => n.Set(this, value);

    public NotifyPropertyValue GetValue(NotifyPropertyA property)
    {
        return new NotifyPropertyValue();
    }
}

public class NotifyPropertyB<TClass, TValue> 
    where  TValue : IEquatable<TValue>
{
    NotifyPropertyB(Expression<Func<TClass, TValue>> getter, Action<TClass, TValue> setter, Expression<Action<TClass>> change)
    {
        _getter = getter.Compile();
        _setter = setter;
        _onPropertyChanged = change.Compile();
    }
    public static NotifyPropertyB<TClass, TValue> Register(Expression<Func<TClass, TValue>> getter, Action<TClass, TValue> setter, Expression<Action<TClass>> change)
        => new NotifyPropertyB<TClass, TValue>(getter, setter, change);

    readonly Func<TClass, TValue> _getter;
    readonly Action<TClass, TValue> _setter;
    readonly Action<TClass> _onPropertyChanged;


    public TValue Get(TClass c) => _getter(c);
    public void Set(TClass c, TValue value)
    {
        var oldValue = _getter(c);
        if (!EqualityComparer<TValue>.Default.Equals(oldValue, value))
        {
            _setter(c, value);
            _onPropertyChanged(c);
        }
    }
}