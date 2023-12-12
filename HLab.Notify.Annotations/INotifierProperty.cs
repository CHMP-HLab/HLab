using System.ComponentModel;

namespace HLab.Notify.Annotations;

public interface INotifierProperty
{
    string Name { get; }
    INotifierPropertyEntry GetNewEntry(INotifyPropertyChanged notifier);
}

public interface INotifierProperty<T> : INotifierProperty
{
    new INotifierPropertyEntry<T> GetNewEntry(INotifyPropertyChanged notifier);
    void AddOneToMany(T oldValue, T newValue, object target);
}