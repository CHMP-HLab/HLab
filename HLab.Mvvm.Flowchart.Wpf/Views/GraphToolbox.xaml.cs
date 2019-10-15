using System.Windows.Controls;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Flowchart.ViewModel;

namespace HLab.Mvvm.Flowchart.Views
{
    /// <summary>
    /// Logique d'interaction pour GraphToolbox.xaml
    /// </summary>
    public partial class GraphToolbox : UserControl, IView<ViewModeDefault,GraphToolboxViewModel>
    {
        public GraphToolbox()
        {
            InitializeComponent();
        }
    }
}
