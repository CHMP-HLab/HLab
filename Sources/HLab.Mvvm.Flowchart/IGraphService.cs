using System.Collections.ObjectModel;
using System.Xml;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Flowchart.Models;

namespace HLab.Mvvm.Flowchart
{
    public interface IViewClassFlowchart : IViewClass { }
    public interface IViewClassGraphContainer : IViewClass { }

    public interface IGraphService
    {
        void LoadXmlAttributes(IGraphElement o, XmlElement r);
        ObservableCollection<IToolGraphBlock> Blocks { get; }
        //void WriteXmlAttributes(XmlWriter w, IGraphElement o);
    }
}