using System;
using System.ComponentModel;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged.NotifyParsers;

namespace HLab.Notify.PropertyChanged
{
    public abstract class NotifierBase : INotifyPropertyChangedWithHelper//, IDisposable
    {
        public INotifyClassHelper ClassHelper { get; }

        protected NotifierBase()
        {
            ClassHelper = NotifyClassHelperBase.GetNewHelper(this);
        }
            
        public event PropertyChangedEventHandler PropertyChanged
        {
            add => ClassHelper.AddHandler(value);
            remove => ClassHelper.RemoveHandler(value);
        }
    }
}