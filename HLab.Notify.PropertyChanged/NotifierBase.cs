using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HLab.Base;
using HLab.DependencyInjection.Annotations;

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