using System;
using System.ComponentModel;
using HLab.Notify.Annotations;

namespace HLab.Notify.PropertyChanged
{
    public interface IChildObject
    {
        INotifyPropertyChangedWithHelper Parent{get;set; }

        public void OnDispose(Action action);
    }
}