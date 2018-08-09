using NetLibrary.Enums;
using System;

namespace NetLibrary.Models
{
    [Serializable()]
    public class NotificationModel
    {
        public NotificationTypes NotificationType { get; set; }
        public string Data { get; set; }
    }
}
