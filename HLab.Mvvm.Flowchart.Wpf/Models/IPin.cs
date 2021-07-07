using System.Collections.ObjectModel;
using System.Windows.Media;

namespace HLab.Mvvm.Flowchart.Models
{
    public enum PinDirection
    { Input, Output}


    public interface IPin : IGraphElement
    {
        ObservableCollection<IPin> LinkedPins { get; }
        bool IsLinked { get; }
        Color Color { get; }
        PinDirection Direction { get; }
        GraphValueType ValueType { get; set; }

        double Value { get; }
        IPinGroup Group { get; set; }
        double GetValue(int n);
    }

}