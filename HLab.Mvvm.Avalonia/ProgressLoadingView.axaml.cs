using System.ComponentModel;
using Avalonia.Controls;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.ReactiveUI;

namespace HLab.Mvvm.Avalonia;

/// <summary>
/// Logique d'interaction pour ProgressLoadingView.xaml
/// </summary>
public partial class ProgressLoadingView : UserControl, IView<DefaultViewMode,ProgressLoadingViewModel>
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