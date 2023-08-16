using System.Windows;
using System.Windows.Controls;
using HLab.Mvvm.Application.Wpf.Update;

namespace HLab.Erp.Core.Update
{
    /// <summary>
    /// Logique d'interaction pour ApplicationUpdate.xaml
    /// </summary>
    public partial class ApplicationUpdateView : UserControl
    {
        public ApplicationUpdateView()
        {
            InitializeComponent();
            DataContextChanged += ApplicationUpdateView_DataContextChanged;
        }

        void ApplicationUpdateView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(DataContext is ApplicationUpdateViewModel viewModel) viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Updated" && ((DataContext as ApplicationUpdateViewModel)?.Model.Updated??false))
            {
                // TODO : Close();
            }
        }

        void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
           //TODO : Close();
        }
    }
}
