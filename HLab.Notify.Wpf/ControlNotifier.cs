using System.ComponentModel;
using System.Windows.Controls;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged.NotifyHelpers;

namespace HLab.Notify.Wpf
{
    public class UserControlNotifier : UserControl, INotifyPropertyChangedWithHelper
    {

        public INotifyClassHelper ClassHelper { get; }

        protected UserControlNotifier()
        {
            ClassHelper = new NotifyClassHelper(this);
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => ClassHelper.AddHandler(value);
            remove => ClassHelper.RemoveHandler(value);
        }

    }
}
