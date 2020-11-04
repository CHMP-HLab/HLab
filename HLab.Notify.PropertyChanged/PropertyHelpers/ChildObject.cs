using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HLab.Notify.PropertyChanged.NotifyParsers;

namespace HLab.Notify.PropertyChanged.PropertyHelpers
{
    public abstract class ChildObject : IChildObject
    {
        //private Action<PropertyChangedEventArgs> _notify;
        private readonly ConfiguratorEntry _configurator;

        protected ChildObject(ConfiguratorEntry configurator)
        {
            _configurator = configurator;
        }


        public INotifyPropertyChangedWithHelper Parent { get; private set; }

        public string Name => _configurator.EventArgs.PropertyName;
        internal void OnPropertyChanged()
        {
            Parent.ClassHelper.OnPropertyChanged(_configurator.EventArgs);
        }

        protected virtual void Configure()
        {
            _configurator.Configure(Parent, this);
        }

        public void SetParent(INotifyPropertyChangedWithHelper parent)
        {
            Parent = parent;
            Configure();
        }

        public void Update() => _configurator.Update(Parent, this);

        public void Dispose()
        {
        }
    }


    public abstract class ChildObjectN<T> : ChildObject, INotifyPropertyChangedWithHelper
    where T : ChildObjectN<T>, INotifyPropertyChanged
    {
        protected class H : H<T> { }
        protected ChildObjectN(ConfiguratorEntry configurator):base(configurator)
        {
            ClassHelper = new NotifyClassHelper(this);
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

        public INotifyClassHelper ClassHelper { get; }
    }

}