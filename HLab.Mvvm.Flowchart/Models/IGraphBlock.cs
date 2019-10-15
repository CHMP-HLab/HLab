using System.Collections.ObjectModel;
using System.Windows;

namespace HLab.Mvvm.Flowchart.Models
{
    public interface IToolGraphBlock : IGraphBlock { }
    public interface IGraphBlock : IGraphElement
    {
        ObservableCollection<IPinGroup> Groups { get; }
        IPinGroup MainLeftGroup { get; }
        IPinGroup MainRightGroup { get; }
        double Left { get; set; }
        double Top { get; set; }

        IGraph Graph { get; set; }

        bool AskForInputPin(GraphValueType type);
        void EndAskForInputPin();

        Point TempLocation { get; set; }

    }
}
