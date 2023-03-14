using System.Windows.Input;

namespace HLab.Notify.Annotations;

public interface INotifyCommand : ICommand
{
    public string IconPath { get; set; }
    public string ToolTipText { get; set; }

    public void CheckCanExecute();
}