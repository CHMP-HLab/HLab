using Avalonia.Controls;
using Avalonia.Interactivity;

namespace HLab.Bugs.Avalonia
{
    /// <summary>
    /// Logique d'interaction pour BugReportView.xaml
    /// </summary>
    public partial class BugReportView : Window
    {
        public BugReportView()
        {
            InitializeComponent();
        }

        void ButtonOk_OnClick(object sender, RoutedEventArgs e)
        {

        }

        void ButtonReopen_OnClick(object sender, RoutedEventArgs e)
        {

        }

        void ButtonShowDetail_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
