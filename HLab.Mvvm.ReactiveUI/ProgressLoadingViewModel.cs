using HLab.Core.Annotations;
using ReactiveUI;

namespace HLab.Mvvm.ReactiveUI;

public class ProgressLoadingViewModel : ViewModel
{
    public ProgressLoadingViewModel(IMessagesService msg)
    {
        msg.Subscribe<ProgressMessage>(OnProgress);
    }


    public void OnProgress(ProgressMessage msg)
    {
        Progress = msg.Progress;
        ProgressText = msg.Text;
    }

    public double Progress
    {
        get => _progress;
        set => this.RaiseAndSetIfChanged(ref _progress, value);
    }
    double _progress;

    public string ProgressText
    {
        get => _progressText;
        set => this.RaiseAndSetIfChanged(ref _progressText, value);
    }
    string _progressText = string.Empty;

    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }
    string _title = string.Empty;
}
