using System.ComponentModel;
using HLab.Notify.PropertyChanged.NotifyHelpers;

namespace HLab.Notify.PropertyChanged;

public abstract class NotifierBase : INotifyPropertyChangedWithHelper
{
    public INotifyClassHelper ClassHelper { get; }

    protected NotifierBase()
    {
        ClassHelper = new NotifyClassHelper(this);
    }
            
    public event PropertyChangedEventHandler PropertyChanged
    {
        add => ClassHelper.AddHandler(value);
        remove => ClassHelper.RemoveHandler(value);
    }
}