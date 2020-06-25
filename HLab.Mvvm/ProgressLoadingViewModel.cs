using HLab.Core;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Notify.PropertyChanged;


namespace HLab.Mvvm
{
    using H = NotifyHelper<ProgressLoadingViewModel>;

    public class ProgressMessage
    {
        public ProgressMessage(double progress, string text)
        {
            Progress = progress;
            Text = text;
        }

        public double Progress { get; }
        public string Text { get; }
    }

    public class ProgressLoadingViewModel : ViewModel
    {
        public ProgressLoadingViewModel()
        {
            H.Initialize(this);
        }


        [Import]
        public void Import(IMessageBus msg)
        {
            msg.Subscribe<ProgressMessage>(OnProgress);
        }

        public void OnProgress(ProgressMessage msg)
        {
            Progress = msg.Progress;
            ProgressText = msg.Text;
        }

        private readonly IProperty<double> _progress = H.Property<double>();
        public double Progress
        {
            get => _progress.Get();
            set => _progress.Set(value);
        }

        private readonly IProperty<string> _progressText
            = H.Property<string>(nameof(ProgressText), c => c.Set(e => ""));
        public string ProgressText
        {
            get => _progressText.Get();
            set => _progressText.Set(value);
        }

        private readonly IProperty<string> _title
            = H.Property<string>(nameof(Title), c => c.Set(e => ""));
        public string Title
        {
            get => _title.Get();
            set => _title.Set(value);
        }
    }
}
