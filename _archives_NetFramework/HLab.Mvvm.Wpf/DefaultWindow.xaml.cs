using System.Windows.Input;

namespace HLab.Mvvm.Wpf
{
    /// <summary>
    /// Logique d'interaction pour DefaultWindow.xaml
    /// </summary>
    public partial class DefaultWindow
    {
        public DefaultWindow()
        {
            InitializeComponent();
        }

        private void DefaultWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
