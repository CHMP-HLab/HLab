using System.Windows.Input;

namespace HLab.Erp.Core
{
    public interface IMenuService
    {
        bool RegisterMenu(string path, object header, ICommand cmd, string getIcon);
    }
}
