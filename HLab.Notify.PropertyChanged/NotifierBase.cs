using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HLab.Notify.PropertyChanged
{
    public abstract class NotifierBase : INotifyPropertyChanged
    {

        protected INotifyClassParser NotifyClassParser;

        protected NotifierBase()
        {
            NotifyClassParser = NotifyFactory.GetParserUninitialized(this);
        }
            
        public event PropertyChangedEventHandler PropertyChanged
        {
            add => NotifyClassParser.AddHandler(value);
            remove => NotifyClassParser.RemoveHandler(value);
        }
        //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        //}

        //protected void OnPropertyChanged(PropertyChangedEventArgs args)
        //{
        //        PropertyChanged?.Invoke(this,args);
        //}

        //protected void Initialize<T>() where T : class => NotifyHelper<T>.Initialize(this as T,OnPropertyChanged);
    }
}