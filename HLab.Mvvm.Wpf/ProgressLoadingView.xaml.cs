using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.ReactiveUI;

namespace HLab.Mvvm
{
    /// <summary>
    /// Logique d'interaction pour ProgressLoadingView.xaml
    /// </summary>
    public partial class ProgressLoadingView : UserControl, IView<ViewModeDefault,ProgressLoadingViewModel>
    {
        public ProgressLoadingView()
        {
            InitializeComponent();
        }

        readonly BackgroundWorker _worker = new BackgroundWorker();
        public async void Run(Action work)
        {
            await Task.Run(work);
        }
    }
}
