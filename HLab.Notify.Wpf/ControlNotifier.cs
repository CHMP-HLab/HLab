using System.ComponentModel;
using System.Windows.Controls;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;
using HLab.Notify.PropertyChanged.NotifyParsers;

namespace HLab.Notify.Wpf
{
    public class UserControlNotifier : UserControl, INotifyPropertyChangedWithHelper
    {

         public INotifyClassHelper ClassHelper { get; }

        protected UserControlNotifier()
        {
            ClassHelper = NotifyClassHelper.GetNewHelper(this);
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => ClassHelper.AddHandler(value);
            remove => ClassHelper.RemoveHandler(value);
        }

    }
}
