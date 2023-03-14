using HLab.Core.Annotations;
using HLab.Mvvm.Legacy;
using HLab.Notify.PropertyChanged;


namespace HLab.Mvvm
{
    using H = H<ProgressLoadingViewModel>;

    public class ProgressLoadingViewModel : ViewModel
    {
        public ProgressLoadingViewModel(IMessagesService msg)
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

        readonly IProperty<double> _progress = H.Property<double>();

        public string ProgressText
        {
            get => _progressText.Get();
            set => _progressText.Set(value);
        }

        readonly IProperty<string> _progressText
            = H.Property<string>( c => c.Set(e => ""));

        public string Title
        {
            get => _title.Get();
            set => _title.Set(value);
        }

        readonly IProperty<string> _title
            = H.Property<string>( c => c.Set(e => ""));
    }
}
