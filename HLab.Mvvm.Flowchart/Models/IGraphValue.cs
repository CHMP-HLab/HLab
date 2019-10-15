using System.Windows.Media;

namespace HLab.Mvvm.Flowchart.Models
{
    public interface IGraphValue
    {
        double Value { get; set; }

        GraphValueType Type { get; }
    }

    public class GraphValueType
    {
        public string Id { get; set; }
        public Color Color { get; set; }
        public string IconName { get; set; }
    }
}
