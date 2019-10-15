using System;
using System.Collections.ObjectModel;

namespace HLab.Mvvm.Flowchart.Models
{
    public interface IGraph : IGraphElement
    {
        event EventHandler BlocksLoaded; 
        ObservableCollection<IGraphBlock> Blocks { get; }
        void AddBlock(IGraphBlock block);
        void Load(string doc);
    }
}
