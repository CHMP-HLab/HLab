using System;
using System.Threading.Tasks;
using System.Windows.Input;
using HLab.Notify.PropertyChanged.PropertyHelpers;

namespace HLab.Notify.PropertyChanged;

public interface ITrigger :IChildObject {}

public class NotifyTrigger : ChildObject, ITrigger
{
    public NotifyTrigger(PropertyActivator configurator) : base(configurator)
    {
    }
}

public class NCommand : INotifyCommand
{
    readonly Action _execute = null;
    readonly Func<Task> _executeAsync = null;
    readonly Func<object,bool> _canExecuteFunc = null;
    bool _canExecute = true;

    public NCommand(Action execute, Func<object,bool> canExecuteFunc = null)
    {
        _execute = execute;
        _canExecuteFunc = canExecuteFunc;
    }
    public NCommand(Func<Task> execute, Func<object,bool> canExecuteFunc = null)
    {
        _executeAsync = execute;
        _canExecuteFunc = canExecuteFunc;
    }

    public bool CanExecute(object parameter=null)
    {
        //if (_executing) return false;

        if (_canExecuteFunc == null) return _canExecute;
        return _canExecuteFunc(parameter);
    }

    async void ICommand.Execute(object parameter)
    {
        if (_executeAsync != null)
            await _executeAsync().ConfigureAwait(true);
        else
            _execute?.Invoke();
    }

    public event EventHandler CanExecuteChanged;
    public string IconPath { get; set; }
    public string ToolTipText { get; set; }

    public void CheckCanExecute()
    {
        var old = _canExecute;
        _canExecute = _canExecuteFunc(null);
        if(old!=_canExecute) CanExecuteChanged?.Invoke(this,new EventArgs());
    }
}

public class NCommand<T> : INotifyCommand
{
    readonly Action<T> _execute = null;
    readonly Func<T,Task> _executeAsync = null;
    readonly Func<object,bool> _canExecuteFunc = null;
    bool _canExecute = true;

    public NCommand(Action<T> execute, Func<object,bool> canExecuteFunc = null)
    {
        _execute = execute;
        _canExecuteFunc = canExecuteFunc;
    }
    public NCommand(Func<T,Task> execute, Func<object,bool> canExecuteFunc = null)
    {
        _executeAsync = execute;
        _canExecuteFunc = canExecuteFunc;
    }

    public bool CanExecute(object parameter=null)
    {
        //if (_executing) return false;

        if (_canExecuteFunc == null) return _canExecute;
        return _canExecuteFunc(parameter);
    }

    async void ICommand.Execute(object parameter)
    {
        if (parameter is T p)
        {
            if (_executeAsync != null)
                await _executeAsync(p).ConfigureAwait(true);
            else
                _execute?.Invoke(p);
        }
        else
        {
            throw new InvalidCastException();
        }
    }

    public event EventHandler CanExecuteChanged;
    public string IconPath { get; set; }
    public string ToolTipText { get; set; }

    public void CheckCanExecute()
    {
        var old = _canExecute;
        _canExecute = _canExecuteFunc(null);
        if(old!=_canExecute) CanExecuteChanged?.Invoke(this,new EventArgs());
    }
}

public class CommandPropertyHolder : ChildObject, ICommand
{
    Action<object> _execute = null;
    Func<object,Task> _executeAsync = null;
    Func<object,bool> _canExecuteFunc = (o) => true;
    bool _canExecute = true;
    public string IconPath { get; set; }
    public string ToolTipText { get; set; }


    public void SetAction(Action<object> execute) => _execute = execute;
    public void SetAction(Action execute) => _execute = o => execute();

    public void SetAction(Func<object, Task> execute) => _executeAsync = async o =>
    {
        _executing = true;
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        await execute(o).ConfigureAwait(true);
        _executing = false;
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    };

    public void SetAction(Func<Task> execute) => _executeAsync = async  o =>
    {
        _executing = true;
        CanExecuteChanged?.Invoke(this,EventArgs.Empty);
        await execute().ConfigureAwait(true);
        _executing = false;
        CanExecuteChanged?.Invoke(this,EventArgs.Empty);
    };

    volatile bool _executing = false;

    public CommandPropertyHolder(PropertyActivator configurator):base(configurator) {  }

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
        //if (_canExecute == value) return value;

        _canExecute = value;

        NotifyHelper.EventHandlerService.Invoke(CanExecuteChanged,this,new EventArgs());

        return value;
    }

    public bool SetCanExecute()
    {
        return SetCanExecute(CanExecute());
    }

    public bool SetCanExecute(object arg)
    {
        return SetCanExecute(CanExecute(arg));
    }

    public bool CanExecute(object parameter=null)
    {
        if (_executing) return false;

        if (_canExecuteFunc != null) 
        {
            _canExecute = _canExecuteFunc(parameter);
        }
        return _canExecute;
    }

    async void ICommand.Execute(object parameter)
    {
        if (_executeAsync != null)
            await _executeAsync(parameter).ConfigureAwait(true);
        else
        {
            _ = _execute ?? throw new NullReferenceException("Child Command not initialized");
            _execute.Invoke(parameter);
        }
    }
    public void CheckCanExecute()
    {
        var old = _canExecute;
        _canExecute = _canExecuteFunc(null);
        if (old != _canExecute) CanExecuteChanged?.Invoke(this, new EventArgs());
    }

    public event EventHandler CanExecuteChanged;
}