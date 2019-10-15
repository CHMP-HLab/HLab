using System;
using System.ComponentModel;

namespace HLab.Notify.PropertyChanged
{
    public abstract class ChildObject : IChildObject
    {
        private Action<PropertyChangedEventArgs> _notify;
        protected INotifyClassParser Parser;

        protected ConfiguratorEntry Configurator { get; set; }

        public object Parent { get; private set; }

        public string Name => Configurator.EventArgs.PropertyName;
        internal void OnPropertyChanged()
        {
            _notify?.Invoke(Configurator.EventArgs);
        }

        protected abstract void Configure();

        public void SetParent(object parent, INotifyClassParser parser, Action<PropertyChangedEventArgs> notifyAction)
        {
            Parent = parent;
            Parser = parser;
            _notify = notifyAction;
            Configure();
        }
    }
}