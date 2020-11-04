using System;
using System.ComponentModel;
using HLab.Notify.PropertyChanged.NotifyParsers;

namespace HLab.Notify.PropertyChanged
{
    public abstract class NotifierBase : INotifyPropertyChangedWithHelper, IDisposable
    {
        public INotifyClassHelper ClassHelper { get; }

        protected NotifierBase()
        {
            ClassHelper = NotifyClassHelper.GetNewHelper(this);
        }
            
        public event PropertyChangedEventHandler PropertyChanged
        {
            add => ClassHelper.AddHandler(value);
            remove => ClassHelper.RemoveHandler(value);
        }

        ~NotifierBase()
        {
            Dispose(false);
        }

        private void ReleaseUnmanagedResources()
        {
            // TODO release unmanaged resources here
        }

        protected virtual void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                ClassHelper?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}