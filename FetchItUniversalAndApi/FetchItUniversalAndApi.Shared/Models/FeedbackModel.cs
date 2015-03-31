using System;
using System.Collections.Generic;
using System.Text;

namespace FetchItUniversalAndApi.Models
{
    public partial class FeedbackModel
    {
        public int FeedbackId { get; set; }
        public byte FeedbackRating { get; set; }
        public string FK_FeedbackNotificationReferenceId { get; set; }
        public string FeedbackComment { get; set; }
        public int FK_FeedbackForTask { get; set; }
        public int FK_FeedbackStatus { get; set; }

        public virtual FeedbackStatusModel FeedbackStatus { get; set; }
        public virtual TaskModel Task { get; set; }
    }
}
