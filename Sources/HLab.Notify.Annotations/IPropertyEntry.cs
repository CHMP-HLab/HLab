using System;
using System.ComponentModel;

namespace HLab.Notify.Annotations
{
    public interface IPropertyEntry// : IDisposable: INotifyPropertyChanged
    {
        event EventHandler<ExtendedPropertyChangedEventArgs> ExtendedPropertyChanged;

        void Link(EventHandler<ExtendedPropertyChangedEventArgs> handler);
        void Unlink(EventHandler<ExtendedPropertyChangedEventArgs> handler);

        void TargetPropertyChanged(object sender, PropertyChangedEventArgs args);

        ITriggerEntry GetTrigger(EventHandler<ExtendedPropertyChangedEventArgs> handler);
        ITriggerEntry GetTrigger(TriggerPath path, EventHandler<ExtendedPropertyChangedEventArgs> handler);

        bool Linked { get; }

        string Name { get; }
        void InitialRegisterValue(Type type);
    }
}