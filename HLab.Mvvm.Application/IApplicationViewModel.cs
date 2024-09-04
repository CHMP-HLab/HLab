using System.Windows.Input;

namespace HLab.Mvvm.Application
{
    public interface IApplicationViewModel
    {
        object Menu { get; }
        ICommand Exit { get; }
    }
}
