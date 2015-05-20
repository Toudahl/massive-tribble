using System.Collections.Generic;

namespace FetchItUniversalAndApi.Models
{
    public partial class NotificationStatusModel
    {
        public NotificationStatusModel()
        {
            this.Notifications = new HashSet<NotificationModel>();
        }

        public int NotificationStatusId { get; set; }
        public string NotificationStatus { get; set; }

        public virtual ICollection<NotificationModel> Notifications { get; set; }
    }
}
