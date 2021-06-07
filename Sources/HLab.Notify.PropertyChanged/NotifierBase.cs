using System;
using System.ComponentModel;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged.NotifyHelpers;

namespace HLab.Notify.PropertyChanged
{
    public abstract class NotifierBase : INotifyPropertyChangedWithHelper//, IDisposable
    {
        private INotifyClassHelper _classHelper;
        public INotifyClassHelper ClassHelper => _classHelper ??= NotifyClassHelperBase.GetNewHelper(this);

        protected NotifierBase()
        {
            _classHelper = NotifyClassHelperBase.GetNewHelper(this);
        }
            
        public event PropertyChangedEventHandler PropertyChanged
        {
            add => ClassHelper.AddHandler(value);
            remove => ClassHelper.RemoveHandler(value);
        }
    }
}