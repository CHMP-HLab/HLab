using System.Collections.ObjectModel;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Flowchart.Models;

namespace HLab.Mvvm.Flowchart.ViewModel
{
    public interface IGraphViewModel
    {
        ObservableCollection<IGraphBlock> Blocks { get; }
        IMvvmService MvvmService { get; }
        IGraphService GraphService { get; }

        void Select(IGraphBlock block);
    }
}
