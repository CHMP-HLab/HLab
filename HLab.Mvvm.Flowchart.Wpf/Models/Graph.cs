using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Flowchart.Models
{
    public class Graph : GraphElement, IGraph
    {
        public event EventHandler BlocksLoaded;

        readonly GraphService _graphService;
        public Graph(GraphService graphService) 
        {
            _graphService = graphService;
            Id = "#";
        }

        [DataMember] public ObservableCollection<IGraphBlock> Blocks { get; } = new ObservableCollection<IGraphBlock>();  // => N.Get(() => );
        public void Load(string s)
        {
            var doc = new XmlDocument();
            doc.LoadXml(s);
            foreach (var node in doc.ChildNodes)
            {
                if (node is not XmlElement e) continue;
                if (e.Name == GetType().Name)
                {
                    _graphService.LoadXmlAttributes(this,e);
                }
            }
            BlocksLoaded?.Invoke(this, EventArgs.Empty);
        }


        public void AddBlock(IGraphBlock block)
        {
            if (string.IsNullOrEmpty(block.Id))
            {
                var blocks = Blocks.Where(p => p.Id.StartsWith("#")).ToList();
                block.Id = "#" + (blocks.Count == 0 ? 0 : blocks.Max(b => int.Parse(b.Id.TrimStart('#'))) + 1);

            }
            block.Graph = this;
            Blocks.Add(block);
        }

    }
}