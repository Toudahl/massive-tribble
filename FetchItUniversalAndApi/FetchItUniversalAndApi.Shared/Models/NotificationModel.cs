using System;
using System.Collections.Generic;
using System.Text;

namespace FetchItUniversalAndApi.Models
{
    public partial class NotificationModel
    {
        public int NotificationId { get; set; }
        public string FK_NotificationReferenceId { get; set; }
        public int FK_NotificationFrom { get; set; }
        public int FK_NotificationTo { get; set; }
        public string NotificationContent { get; set; }
        public int FK_NotificationStatus { get; set; }
        public System.DateTime NotificationSent { get; set; }
        public Nullable<System.DateTime> NotificationRead { get; set; }

        public virtual NotificationStatusModel NotificationStatus { get; set; }
        public virtual ProfileModel FromProfile { get; set; }
        public virtual ProfileModel ToProfile { get; set; }
    }
}
