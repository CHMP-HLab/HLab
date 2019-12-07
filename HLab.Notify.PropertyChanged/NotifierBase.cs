using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HLab.Notify.PropertyChanged
{
    public abstract class NotifierBase : INotifyPropertyChanged
    {
            
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public void OnPropertyChanged(PropertyChangedEventArgs args)
        {
                PropertyChanged?.Invoke(this,args);
        }
    }
}