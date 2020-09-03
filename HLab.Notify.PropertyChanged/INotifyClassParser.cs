using System.Collections.Generic;
using System.ComponentModel;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged.NotifyParsers;

namespace HLab.Notify.PropertyChanged
{
    public delegate void ExtendedPropertyChangedEventHandler(object sender, ExtendedPropertyChangedEventArgs e);
    public interface INotifyClassHelper
    {
        IPropertyEntry GetPropertyEntry(string name);
        ITriggerEntry GetTrigger(TriggerPath path, ExtendedPropertyChangedEventHandler handler);
        IEnumerable<IPropertyEntry> LinkedProperties();
        IEnumerable<IPropertyEntry> Properties();
        void Initialize<T>() where T : class, INotifyPropertyChanged;
        void AddHandler(PropertyChangedEventHandler value);
        void RemoveHandler(PropertyChangedEventHandler value);
        void OnPropertyChanged(PropertyChangedEventArgs args);
    }
}