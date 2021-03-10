using System.Xml.Serialization;

namespace HLab.Mvvm.Flowchart.Models
{
    public interface IGraphElement : IXmlSerializable
    {
        IGraphElement Parent { get; set; }
        string Id { get; set; }
        string Caption { get; set; }
        string IconPath { get; set; }
    }
}