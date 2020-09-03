using System;
using System.ComponentModel;
using HLab.Notify.Annotations;

namespace HLab.Notify.PropertyChanged
{
    public interface IPropertyEntry //: INotifyPropertyChanged
    {
        event ExtendedPropertyChangedEventHandler ExtendedPropertyChanged;

        void Link(ExtendedPropertyChangedEventHandler handler);
        void Unlink(ExtendedPropertyChangedEventHandler handler);

        void TargetPropertyChanged(object sender, PropertyChangedEventArgs args);

        ITriggerEntry GetTrigger(TriggerPath path, ExtendedPropertyChangedEventHandler handler);

        bool Linked { get; }

        string Name { get; }
        void InitialRegisterValue(Type type);
    }
}