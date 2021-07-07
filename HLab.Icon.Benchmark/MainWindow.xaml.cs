using System;
using System.Windows;
using System.Windows.Controls;

namespace HLab.Icon.Benchmark
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public TextBlock Text => TextBlock;

        public void Do(Action action)
        {
            Dispatcher.BeginInvoke(action);
            ShowDialog();
        }
    }
}
