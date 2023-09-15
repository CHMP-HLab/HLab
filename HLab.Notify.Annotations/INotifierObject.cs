using System.ComponentModel;

namespace HLab.Notify.Annotations;

public interface INotifierObject : INotifyPropertyChanged
{
    INotifier GetNotifier();
}