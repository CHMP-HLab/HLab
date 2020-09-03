using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HLab.Notify.PropertyChanged.PropertyHelpers
{
    public abstract class ChildObject : IChildObject
    {
        //private Action<PropertyChangedEventArgs> _notify;
        private readonly ConfiguratorEntry _configurator;

        protected INotifyClassHelper ClassHelper;

        protected ChildObject(ConfiguratorEntry configurator)
        {
            _configurator = configurator;
        }


        public object Parent { get; private set; }

        public string Name => _configurator.EventArgs.PropertyName;
        internal void OnPropertyChanged()
        {
            ClassHelper.OnPropertyChanged(_configurator.EventArgs);
        }

        protected virtual void Configure()
        {
            _configurator.Configure(Parent,ClassHelper, this);
        }

        public void SetParent(object parent, INotifyClassHelper helper)
        {
            Parent = parent;
            ClassHelper = helper;
            Configure();
        }

        public void Update() => _configurator.Update(Parent, this);
    }


    public abstract class ChildObjectN<T> : ChildObject, INotifyPropertyChanged
    where T : ChildObjectN<T>, INotifyPropertyChanged
    {
        protected class H : H<T> { }
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