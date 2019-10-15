using System;
using System.ComponentModel;
using System.Windows.Input;

namespace HLab.Notify.PropertyChanged
{
    public class NotifyCommand<TClass> : NotifyCommand
        where TClass : class//, INotifyPropertyChanged
    {
        private readonly PropertyCache<TClass>.ConfiguratorEntry<NotifyCommand<TClass>>
            _configurator;
        public NotifyCommand(string name, NotifyConfiguratorFactory<TClass,NotifyCommand<TClass>> configurator)
        {
            if (configurator != null)
                _configurator =
                    PropertyCache<TClass>.Get(name,
                        () => configurator(new NotifyConfigurator<TClass, NotifyCommand<TClass>>())
                            .Compile());
        }

        protected override void Configure()
        {
            _configurator.Configure((TClass)Parent,Parser, this);
        }
    }

    public abstract class NotifyCommand : ChildObject, ICommand
    {
        private Action<object> _execute = null;
        private Func<object,bool> _canExecuteFunc = null;
        private bool _canExecute = true;

        public void SetAction(Action execute) => _execute = o => execute();
        public void SetAction(Action<object> execute) => _execute = execute;
        public void SetCanExecute(Func<bool> func)
        {
            _canExecuteFunc = o => func();
        }
        public void SetCanExecute(Func<object,bool> func)
        {
            _canExecuteFunc = func;
        }

        public bool SetCanExecute(bool value)
        {
            if (_canExecute == value) return value;

            _canExecute = value;

            CanExecuteChanged?.Invoke(this, new EventArgs());

            return value;
        }

        public bool SetCanExecute()
        {
            return SetCanExecute(CanExecute());
        }

        public bool CanExecute(object parameter=null)
        {
            if (_canExecuteFunc == null) return _canExecute;
            return _canExecuteFunc(parameter);
        }


        public void Execute(object parameter=null)
        {
            _execute?.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }

}