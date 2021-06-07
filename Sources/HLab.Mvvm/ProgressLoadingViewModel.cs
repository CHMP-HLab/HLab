using HLab.Core;
using HLab.Core.Annotations;
using HLab.Notify.PropertyChanged;


namespace HLab.Mvvm
{
    using H = H<ProgressLoadingViewModel>;

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
        public ProgressLoadingViewModel(IMessageBus msg)
        {
            H.Initialize(this);
            msg.Subscribe<ProgressMessage>(OnProgress);
        }


        public void OnProgress(ProgressMessage msg)
        {
            Progress = msg.Progress;
            ProgressText = msg.Text;
        }

        public double Progress
        {
            get => _progress.Get();
            set => _progress.Set(value);
        }
        private readonly IProperty<double> _progress = H.Property<double>();

        public string ProgressText
        {
            get => _progressText.Get();
            set => _progressText.Set(value);
        }
        private readonly IProperty<string> _progressText
            = H.Property<string>( c => c.Set(e => ""));

        public string Title
        {
            get => _title.Get();
            set => _title.Set(value);
        }
        private readonly IProperty<string> _title
            = H.Property<string>( c => c.Set(e => ""));
    }
}
