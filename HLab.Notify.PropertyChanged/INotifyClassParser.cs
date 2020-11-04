using System;
using System.Collections.Generic;
using System.ComponentModel;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged.NotifyParsers;

namespace HLab.Notify.PropertyChanged
{
    public interface INotifyClassHelper : IDisposable
    {
        IPropertyEntry GetPropertyEntry(string name);
        ITriggerEntry GetTrigger(TriggerPath path, EventHandler<ExtendedPropertyChangedEventArgs> handler);
        IEnumerable<IPropertyEntry> LinkedProperties();
        IEnumerable<IPropertyEntry> Properties();
        void Initialize<T>() where T : class, INotifyPropertyChangedWithHelper;
        void AddHandler(PropertyChangedEventHandler value);
        void RemoveHandler(PropertyChangedEventHandler value);
        void OnPropertyChanged(PropertyChangedEventArgs args);
    }

    public interface INotifyPropertyChangedWithHelper : INotifyPropertyChanged
    {
        INotifyClassHelper ClassHelper { get; }
    }
}