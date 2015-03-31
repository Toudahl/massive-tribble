using System;
using System.Collections.Generic;
using System.Text;

namespace FetchItUniversalAndApi.Models
{
    public partial class FeedbackStatusModel
    {
        public FeedbackStatusModel()
        {
            this.Feedbacks = new HashSet<FeedbackModel>();
        }

        public int FeedbackStatusId { get; set; }
        public string FeedbackStatus { get; set; }

        public virtual ICollection<FeedbackModel> Feedbacks { get; set; }
    }
}
