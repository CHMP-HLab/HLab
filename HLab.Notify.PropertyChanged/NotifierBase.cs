using System.ComponentModel;
using System.Runtime.CompilerServices;
using HLab.Notify.PropertyChanged.NotifyParsers;

namespace HLab.Notify.PropertyChanged
{
    public abstract class NotifierBase : INotifyPropertyChanged
    {

        protected INotifyClassHelper Parser;

        protected NotifierBase()
        {
            Parser = NotifyClassHelper.GetParserUninitialized(this);
        }
            
        public event PropertyChangedEventHandler PropertyChanged
        {
            add => Parser.AddHandler(value);
            remove => Parser.RemoveHandler(value);
        }
    }
}