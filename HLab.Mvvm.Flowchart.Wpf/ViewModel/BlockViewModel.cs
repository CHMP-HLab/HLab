using System;
using System.Windows;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Flowchart.Models;
using HLab.Notify.PropertyChanged;

namespace HLab.Mvvm.Flowchart.ViewModel
{
    using H = H<BlockViewModel>;
    public class BlockViewModel : ViewModel<IGraphBlock>, IBlockViewModel
    {
        public BlockViewModel() => H.Initialize(this);

        public IGraphViewModel GraphViewModel
        {
            get => _graphViewModel.Get();
            set => _graphViewModel.Set(value);
        }

        readonly IProperty<IGraphViewModel> _graphViewModel = H.Property<IGraphViewModel>(c => c.Default((IGraphViewModel)default));


        public IObservableFilter<IPinGroup> LeftGroups { get; } = H.Filter<IPinGroup>(c => c
            .AddFilter(g => g.Location == PinLocation.Left).Link(e => e.Model.Groups)
        );

        public IObservableFilter<IPinGroup> RightGroups { get; } = H.Filter<IPinGroup>(c => c
                .AddFilter(g => g.Location == PinLocation.Right).Link(e => e.Model.Groups)
        );


        public Thickness Margin => _margin.Get();

        readonly IProperty<Thickness> _margin = H.Property<Thickness>(c => c
            .On(e => e.Model.Left)
            .On(e => e.Model.Top)
            .Set(
                e => new Thickness(e.Model.Left, e.Model.Top, 0, 0)
            ));


        public bool Selected
        {
            get => _selected.Get();
            set => _selected.Set(value);
        }

        readonly IProperty<bool> _selected = H.Property<bool>(c => c.Default((bool)default));


        public void Select()
        {
            GraphViewModel?.Select(Model);
        }

        public Visibility SelectedVisibility => _selectedVisibility.Get();

        readonly IProperty<Visibility> _selectedVisibility = H.Property<Visibility>(c => c
            .On(e => e.Selected)
            .Set(e => e.Selected ? Visibility.Visible : Visibility.Collapsed)
        );

        public Type ViewMode => _viewMode.Get();

        readonly IProperty<Type> _viewMode = H.Property<Type>(c => c
            .On(e => e.Selected)
            .Set(e => e.Selected ? typeof(ViewModeEdit) : typeof(ViewModeDefault)));


        public void SetLocation(double left, double top)
        {
            Model.Left = left;
            Model.Top = top;
        }

        public void SetDraggedPin(PinViewModel pvm)
        {
            if(pvm==null)
                Model.EndAskForInputPin();
            else
                Model.AskForInputPin(pvm.Model.ValueType);
        }
    }
}
