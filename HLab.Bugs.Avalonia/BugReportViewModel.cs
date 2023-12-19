using System.Windows.Input;

namespace HLab.Bugs.Avalonia
{
    public class BugReportViewModel
    {
        public object Foreground { get; }
        public object Login { get; }
        public object Message { get; }

        public ICommand LoginCommand { get; }
    }
}
