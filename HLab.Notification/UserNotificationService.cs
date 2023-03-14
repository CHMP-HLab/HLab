using HLab.Core.Annotations;

using System;

namespace HLab.UserNotification
{
    public interface IUserNotificationService
    {
        void AddMenu(int v1, string v2, string v3, Action showControl);
        event Action<object, object> Click;
        void SetIcon(string icon, int i);

        public string ToolTipText { get; set; }
        void Show();
    }

    public class UserNotificationServiceBootloader : IBootloader
    {
        private readonly IUserNotificationService _userNotificationService;

        public UserNotificationServiceBootloader(IUserNotificationService service)
        {
            _userNotificationService = service;
        }

        public void Load(IBootContext bootstrapper)
        {

        }
    }

}
