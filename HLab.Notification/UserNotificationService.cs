using HLab.Core.Annotations;

using System;

namespace HLab.UserNotification
{
    public interface IUserNotificationService
    {

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

    public abstract class UserNotificationService
    {


    }
}
