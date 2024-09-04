using System;
using System.ComponentModel;

namespace HLab.Notify.Annotations;

public interface IPropertyEntry// : IDisposable: INotifyPropertyChanged
{
    event EventHandler<ExtendedPropertyChangedEventArgs> ExtendedPropertyChanged;

    void Link(EventHandler<ExtendedPropertyChangedEventArgs> handler);
    void Unlink(EventHandler<ExtendedPropertyChangedEventArgs> handler);

    void TargetPropertyChanged(object sender, PropertyChangedEventArgs args);

    ITriggerEntry BuildTrigger(EventHandler<ExtendedPropertyChangedEventArgs> handler);
    ITriggerEntry BuildTrigger(TriggerPath path, EventHandler<ExtendedPropertyChangedEventArgs> handler);

    bool IsLinked();
        
    string Name { get; }
        
    void InitialRegisterValue(Type type);
}