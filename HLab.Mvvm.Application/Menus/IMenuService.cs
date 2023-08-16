using System.Windows.Input;

namespace HLab.Mvvm.Application.Menus;

public interface IMenuService
{
    object MainMenu { get; }
    bool RegisterMenu(string path, object header, ICommand cmd, string getIcon);
}
