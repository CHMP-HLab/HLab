using System;
using System.ComponentModel;

namespace HLab.Notify.PropertyChanged
{
    public interface IChildObject
    {
        void SetParent(object parent,INotifyClassParser parser);
    }
}