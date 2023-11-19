using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HLab.Notify.PropertyChanged.NotifyHelpers;

namespace HLab.Notify.PropertyChanged.PropertyHelpers;

public abstract class ChildObject : IChildObject
{
    readonly PropertyActivator _activator;
    readonly List<WeakReference<Action>> _onDispose = new();

    protected ChildObject(PropertyActivator activator)
    {
        _activator = activator;
    }

    ~ChildObject()
    {
        foreach (var onDisposeWeak in _onDispose)
        {
            if (onDisposeWeak.TryGetTarget(out var onDispose))
            {
                onDispose();
            }
        }
    }

    INotifyPropertyChangedWithHelper _parent;
    public INotifyPropertyChangedWithHelper Parent { 
        get => _parent; 
        set {
            _parent = value;
            Activate();
        }
    }

    public string Name => _activator.PropertyName;
    internal void OnPropertyChanged<T>(T oldValue, T newValue)
    {
        var args = new ExtendedPropertyChangedEventArgs(_activator.PropertyName,oldValue,newValue);
        Parent.ClassHelper.OnPropertyChanged(args);
    }

    protected virtual void Activate() => _activator.Activate(Parent, this);

    public void Update() => _activator.Update(this);

    public void OnDispose(Action action)
    {
        _onDispose.Add(new(action));
    }
}


public abstract class ChildObjectN<T> : ChildObject, INotifyPropertyChangedWithHelper
    where T : ChildObjectN<T>, INotifyPropertyChanged
{
    protected class H : H<T> { }
    protected ChildObjectN(PropertyActivator configurator):base(configurator)
    {
        ClassHelper = new NotifyClassHelper(this);
        H.Initialize((T)this);
    }

   
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    public void OnPropertyChanged(PropertyChangedEventArgs args)
    {
        PropertyChanged?.Invoke(this,args);
    }

    public INotifyClassHelper ClassHelper { get; }
}