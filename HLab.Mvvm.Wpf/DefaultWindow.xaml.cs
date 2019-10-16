using System.Windows.Input;

namespace HLab.Mvvm
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
            // MahMetro
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }

    /*
     
    xmlns:controls="clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro"             
    ShowTitleBar="False"
    ShowSystemMenu="True"
 
     */
}
