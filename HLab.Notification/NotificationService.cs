using HLab.Core.Annotations;

using System;

namespace HLab.Notification
{
    public interface INotificationService
    {

    }

    public class NotificationServiceBootloader : IBootloader
    {
        private readonly INotificationService _notificationService;

        public NotificationServiceBootloader(INotificationService service)
        {
            _notificationService = service;
        }

        public void Load(IBootContext bootstrapper)
        {
            
        }
    }

    public abstract class NotificationService
    {


    }
}
