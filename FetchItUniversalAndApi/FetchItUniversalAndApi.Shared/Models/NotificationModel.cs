using System;

namespace FetchItUniversalAndApi.Models
{
    public struct NotificationModel
    {
        public int NotificationId { get; set; }
        public string FK_NotificationReferenceId { get; set; }
        public int FK_NotificationFrom { get; set; }
        public int FK_NotificationTo { get; set; }
        public string NotificationContent { get; set; }
        public int FK_NotificationStatus { get; set; }
        public DateTime NotificationSent { get; set; }
        public DateTime? NotificationRead { get; set; }

        public override string ToString()
        {
            return /*"TODO Missing FromProfile string: "
                    * No its not. Get it from FK_NotificationFrom+*/ NotificationContent;
        }
    }
}
