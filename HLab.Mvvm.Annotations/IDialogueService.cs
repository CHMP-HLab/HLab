using HLab.Core.Annotations;

namespace HLab.Mvvm;

public interface IDialogService : IService
{
    void ShowMessageOk(string text, string caption, string icon);
    bool ShowMessageOkCancel(string text, string caption, string icon);
    bool ShowMessageYesNo(string text, string caption, string icon);
    bool? ShowMessageYesNoCancel(string text, string caption, string icon);
}
