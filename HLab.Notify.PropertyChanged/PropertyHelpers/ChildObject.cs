using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HLab.Notify.PropertyChanged.PropertyHelpers
{
    public abstract class ChildObject : IChildObject
    {
        //private Action<PropertyChangedEventArgs> _notify;
        private readonly ConfiguratorEntry _configurator;

        protected INotifyClassParser Parser;

        protected ChildObject(ConfiguratorEntry configurator)
        {
            _configurator = configurator;
        }


        public object Parent { get; private set; }

        public string Name => _configurator.EventArgs.PropertyName;
        internal void OnPropertyChanged()
        {
            Parser.OnPropertyChanged(_configurator.EventArgs);
        }

        protected virtual void Configure()
        {
            _configurator.Configure(Parent,Parser, this);
        }

        public void SetParent(object parent, INotifyClassParser parser)
        {
            Parent = parent;
            Parser = parser;
            Configure();
        }
    }


    public abstract class ChildObjectN<T> : ChildObject
    where T : ChildObjectN<T>
    {
        protected class H : NotifyHelper<T> { }
        protected ChildObjectN(ConfiguratorEntry configurator):base(configurator)
        {
            H.Initialize((T)this);
        }

   
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this,args);
        }
    }

}