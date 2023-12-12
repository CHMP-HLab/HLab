using System;

namespace HLab.Notify.PropertyChanged;

public interface IChildObject
{
    INotifyPropertyChangedWithHelper Parent{get;set; }

    public void OnDispose(Action action);
}