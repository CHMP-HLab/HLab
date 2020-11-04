using System;
using System.ComponentModel;

namespace HLab.Notify.PropertyChanged
{
    public interface IChildObject : IDisposable
    {
        void SetParent(INotifyPropertyChangedWithHelper parent);
    }
}