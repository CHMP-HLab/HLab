using System.ComponentModel;

namespace HLab.Notify.PropertyChanged
{
    public interface IOnPropertyChanged : INotifyPropertyChanged
    {
        void OnPropertyChanged(PropertyChangedEventArgs args);
    }

}
