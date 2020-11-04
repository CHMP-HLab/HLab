using System.ComponentModel;
using System.Windows.Controls;
using HLab.Notify.PropertyChanged;
using HLab.Notify.PropertyChanged.NotifyParsers;

namespace HLab.Notify.Wpf
{
    public class UserControlNotifier : UserControl, INotifyPropertyChanged
    {

        protected INotifyClassHelper Parser;

        protected UserControlNotifier()
        {
            Parser = NotifyClassHelper.GetHelper(this);
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => Parser.AddHandler(value);
            remove => Parser.RemoveHandler(value);
        }
    }
}
