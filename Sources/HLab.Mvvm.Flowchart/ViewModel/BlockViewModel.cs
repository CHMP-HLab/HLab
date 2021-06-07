using System;
using System.Windows;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Flowchart.Models;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Mvvm.Flowchart.ViewModel
{
    public class BlockViewModel : ViewModel<BlockViewModel,IGraphBlock>, IBlockViewModel
    {
        public IGraphViewModel GraphViewModel
        {
            get => _graphViewModel.Get();
            set => _graphViewModel.Set(value);
        }

        private readonly IProperty<IGraphViewModel> _graphViewModel = H.Property<IGraphViewModel>(c => c.Default(default));


        public ObservableFilter<IPinGroup> LeftGroups => _leftGroups.Get();
        private readonly IProperty<ObservableFilter<IPinGroup>> _leftGroups = H.Property<ObservableFilter<IPinGroup>>(c => c
            .Set(e => new ObservableFilter<IPinGroup>().AddFilter(g => g.Location == PinLocation.Left).Link(() => e.Model.Groups))
        );

        public ObservableFilter<IPinGroup> RightGroups => _rightGroups.Get();
        private readonly IProperty<ObservableFilter<IPinGroup>> _rightGroups = H.Property<ObservableFilter<IPinGroup>>(c => c
            .Set(e => new ObservableFilter<IPinGroup>().AddFilter(g => g.Location == PinLocation.Right).Link(() => e.Model.Groups))
        );


        public Thickness Margin => _margin.Get();
        private readonly IProperty<Thickness> _margin = H.Property<Thickness>(c => c
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

        private readonly IProperty<bool> _selected = H.Property<bool>(c => c.Default(default));


        public void Select()
        {
            GraphViewModel?.Select(Model);
        }

        public Visibility SelectedVisibility => _selectedVisibility.Get();
        private readonly IProperty<Visibility> _selectedVisibility = H.Property<Visibility>(c => c
            .On(e => e.Selected)
            .Set(e => e.Selected ? Visibility.Visible : Visibility.Collapsed)
        );

        public Type ViewMode => _viewMode.Get();
        private readonly IProperty<Type> _viewMode = H.Property<Type>(c => c
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
