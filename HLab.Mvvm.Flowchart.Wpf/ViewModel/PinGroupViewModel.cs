using System.Windows;
using HLab.Mvvm.Flowchart.Models;
using HLab.Notify.PropertyChanged;

namespace HLab.Mvvm.Flowchart.ViewModel
{
    using H = H<PinGroupViewModel>;

    internal class PinGroupViewModel : ViewModel<IPinGroup>
    {
        public PinGroupViewModel() => H.Initialize(this);

        public Visibility OuterVisibility => _outerVisibility.Get();

        readonly IProperty<Visibility> _outerVisibility = H.Property<Visibility>(c => c
            .Set(e => !string.IsNullOrEmpty(e.Model?.Caption) ? Visibility.Visible : Visibility.Collapsed)
            .On(e => e.Model.Caption)
            .Update()
        );

    }
}
