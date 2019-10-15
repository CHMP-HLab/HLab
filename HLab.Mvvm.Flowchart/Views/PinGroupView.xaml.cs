using System.Windows.Controls;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Flowchart.ViewModel;

namespace HLab.Mvvm.Flowchart.Views
{
    /// <summary>
    /// Logique d'interaction pour PinGroupView.xaml
    /// </summary>
    public partial class PinGroupView : UserControl, 
        IView<ViewModeDefault, PinGroupViewModel>,
        IView<ViewModeEdit, PinGroupViewModel>
    {
        public PinGroupView()
        {
            InitializeComponent();
        }
    }
}
