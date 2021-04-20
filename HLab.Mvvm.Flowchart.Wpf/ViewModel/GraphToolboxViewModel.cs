﻿using System.Collections.ObjectModel;
using HLab.Mvvm.Flowchart.Models;
using HLab.Notify.PropertyChanged;

namespace HLab.Mvvm.Flowchart.ViewModel
{
    public class GraphToolboxViewModel : ViewModel<GraphToolboxViewModel>
    {
        private readonly IGraphService _graph;

        public GraphToolboxViewModel(IGraphService graph)
        {
            _graph = graph;
            H<GraphToolboxViewModel>.Initialize(this);
        }

        public ObservableCollection<IToolGraphBlock> Blocks => _blocks.Get();
        private readonly IProperty<ObservableCollection<IToolGraphBlock>> _blocks 
            = H<GraphToolboxViewModel>.Property<ObservableCollection<IToolGraphBlock>>(c => c.Set(e => e._graph.Blocks));

    }
}
