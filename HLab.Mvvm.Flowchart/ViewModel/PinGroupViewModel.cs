using System.Windows;
using HLab.Mvvm.Flowchart.Models;
using HLab.Notify.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Mvvm.Flowchart.ViewModel
{
    class PinGroupViewModel : ViewModel<PinGroupViewModel,IPinGroup>
    {
        public Visibility OuterVisibility => _outerVisibility.Get();
        private readonly IProperty<Visibility> _outerVisibility = H.Property<Visibility>(c => c
            .On(e => e.Model.Caption)
            .Set(e => !string.IsNullOrEmpty(e.Model?.Caption) ? Visibility.Visible : Visibility.Collapsed)
        );

    }
}
