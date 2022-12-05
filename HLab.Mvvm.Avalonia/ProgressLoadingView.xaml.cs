using System.ComponentModel;
using Avalonia.Controls;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Avalonia
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
