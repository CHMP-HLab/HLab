using System;
using System.ComponentModel;

namespace HLab.Notify.Annotations;

public interface INotifierPropertyEntry
{
    event PropertyChangedEventHandler PropertyChanged;
    event PropertyChangedEventHandler RegisterValue;

    bool IsSet { get; }
    void Update(bool alwaysNotify = false);
    INotifierProperty Property { get; }

    TriggerOnEntry Subscribe(PropertyChangedEventHandler handler, TriggerPath path);
    void OnTrigger(object sender, PropertyChangedEventArgs args);
    object GetValue();
    bool Set(object value, bool alwaysNotify, Action<object, object> postUpdateAction = null);
    void SetInitialValue(object value);
}

public interface INotifierPropertyEntry<T> : INotifierPropertyEntry
{
    new INotifierProperty<T> Property { get; }

    new T GetValue();
    T Locate(Func<T, T> getter = null, Action<T,T> postUpdateAction = null);
    T Get(Func<T, T> getter, Action<T,T> postUpdateAction = null);
    bool Set(T value, bool alwaysNotify, Action<T, T> postUpdateAction = null);
    void SetInitialValue(T value);
}