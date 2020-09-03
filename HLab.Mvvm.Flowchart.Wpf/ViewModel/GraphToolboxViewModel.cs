using System.Collections.ObjectModel;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm.Flowchart.Models;
using HLab.Notify.PropertyChanged;

namespace HLab.Mvvm.Flowchart.ViewModel
{
    class GraphToolboxViewModel : ViewModel<GraphToolboxViewModel>
    {
        [Import] private readonly IGraphService _graph;

        public GraphToolboxViewModel() => H<GraphToolboxViewModel>.Initialize(this);

        public ObservableCollection<IToolGraphBlock> Blocks => _blocks.Get();
        private readonly IProperty<ObservableCollection<IToolGraphBlock>> _blocks 
            = H<GraphToolboxViewModel>.Property<ObservableCollection<IToolGraphBlock>>(c => c.Set(e => e._graph.Blocks));

    }
}
