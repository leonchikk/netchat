using NetLibrary.Models;

namespace NetLibrary.EventsArgs
{
    public class ReceiveNotificationEventsArgs
    {
        public NotificationModel Notification { get; set; }

        public ReceiveNotificationEventsArgs(NotificationModel notification)
        {
            Notification = notification;
        }
    }
}
