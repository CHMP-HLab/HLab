using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MouseHooker;

namespace MouseTestWpf
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IMouseHooker _hook = new MouseHookerWinEvent();
        //private Timer _timer;

        private void Callback(object state)
        {
            this.Dispatcher.Invoke(() =>
            {
                TextBlock.Text += "0";
                _hook.Hook();
            });
        }


        public MainWindow()
        {
            InitializeComponent();

            _hook.MouseMove += Hook_MouseMove;

            _hook.Hook();

            //_timer = new Timer(Callback,this,0,1000);
        }
        private void Hook_MouseMove(object sender, HookMouseEventArg e)
        {
            TextBlock.Text = e.Point.X.ToString(CultureInfo.InvariantCulture) + "," + e.Point.Y.ToString(CultureInfo.InvariantCulture);
        }
    }
}
