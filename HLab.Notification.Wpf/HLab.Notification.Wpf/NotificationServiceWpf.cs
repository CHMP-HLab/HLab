using Hardcodet.Wpf.TaskbarNotification;

using System.Drawing;

namespace HLab.Notification.Wpf
{
    public class NotificationServiceWpf : NotificationService
    {
        public string ToolTipText
        {
            get => _toolTipText;
            set
            {
                _toolTipText = value;
                if (_taskbarIcon != null)
                    _taskbarIcon.ToolTipText = value;
            }
        }

        public Icon Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                if(_taskbarIcon!=null)
                    _taskbarIcon.Icon = value;
            }
        }

        private TaskbarIcon _taskbarIcon;
        private Icon _icon;
        private string _toolTipText;

        public void Show()
        {
            _taskbarIcon = new TaskbarIcon
            {
                Icon = Icon,
                ToolTipText = ToolTipText,
                
            };

        }

        public void Notify(string title, string message)
        {
            _taskbarIcon.ShowBalloonTip(title, message, BalloonIcon.None);
        }
        public void NotifyInfo(string title, string message)
        {
            _taskbarIcon.ShowBalloonTip(title, message, BalloonIcon.Info);
        }
        public void NotifyWarning(string title, string message)
        {
            _taskbarIcon.ShowBalloonTip(title, message, BalloonIcon.Warning);
        }
        public void NotifyError(string title, string message)
        {
            _taskbarIcon.ShowBalloonTip(title, message, BalloonIcon.Error);
        }

    }
}
