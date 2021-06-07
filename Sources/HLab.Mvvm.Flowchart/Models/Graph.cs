using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using HLab.DependencyInjection.Annotations;

namespace HLab.Mvvm.Flowchart.Models
{
    public class Graph<T> : GraphElement<T>, IGraph
    where T : Graph<T>
    {
        public event EventHandler BlocksLoaded;

        [Import] private readonly GraphService _graphService;

        [DataMember] public ObservableCollection<IGraphBlock> Blocks { get; } = new ObservableCollection<IGraphBlock>();  // => N.Get(() => );
        public void Load(string s)
        {
            var doc = new XmlDocument();
            doc.LoadXml(s);
            foreach (var node in doc.ChildNodes)
            {
                if (node is XmlElement e)
                {
                    if (e.Name == GetType().Name)
                    {
                        _graphService.LoadXmlAttributes(this,e);
                    }
                }
            }
            BlocksLoaded?.Invoke(this, new EventArgs());
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

        public Graph()
        {
            Id = "#";
        }
    }
}