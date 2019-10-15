using System.ComponentModel;
using HLab.Notify.Annotations;

namespace HLab.Notify.PropertyChanged
{
    public interface IPropertyEntry : INotifyPropertyChanged
    {
        event RegisterValueEventHandler RegisterValue;

        void Link(RegisterValueEventHandler handler);
        void Unlink(RegisterValueEventHandler handler);

        void OnPropertyChanged(object sender, PropertyChangedEventArgs args);

        ITriggerEntry GetTrigger(TriggerPath path, PropertyChangedEventHandler handler);

        bool Linked { get; }

        string Name { get; }
        void InitialRegisterValue();
    }
}