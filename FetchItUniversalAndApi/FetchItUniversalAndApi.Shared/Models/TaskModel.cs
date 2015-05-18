using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FetchItUniversalAndApi.Handlers;

namespace FetchItUniversalAndApi.Models
{
    public partial class TaskModel
    {
        public TaskModel()
        {
            this.Comments = new HashSet<CommentModel>();
            this.Feedbacks = new HashSet<FeedbackModel>();
            this.Issues = new HashSet<IssueModel>();
            this.Payments = new HashSet<PaymentModel>();
            this.TaskLocationInfos = new HashSet<TaskLocationInfoModel>();
        }

        public int TaskId { get; set; }
        public string FK_TaskNotificationReferenceId { get; set; }
        public int FK_TaskMaster { get; set; }
        public int? FK_TaskFetcher { get; set; }
        public string TaskDescription { get; set; }
        public System.DateTime TaskTimeCreated { get; set; }
        public System.DateTime TaskDeadline { get; set; }
        public decimal TaskItemPrice { get; set; }
        public decimal TaskFee { get; set; }
        public int FK_TaskStatus { get; set; }
        public string TaskEndPointAddress { get; set; }

        public virtual ICollection<CommentModel> Comments { get; set; }
        public virtual ICollection<FeedbackModel> Feedbacks { get; set; }
        public virtual ICollection<IssueModel> Issues { get; set; }
        public virtual ICollection<PaymentModel> Payments { get; set; }
        public virtual ProfileModel FetcherProfile { get; set; }
        public virtual ProfileModel MasterProfile { get; set; }
        public virtual ICollection<TaskLocationInfoModel> TaskLocationInfos { get; set; }
        public virtual TaskStatusModel TaskStatus { get; set; }

        //Author: Lárus Þór Kick-Assness
        public override string ToString()
        {
            //Using the ProfileHandler.GetInstance() without initializing it as a property inside here doesn't work.
            //string testreturnString = ProfileHandler.GetInstance().AllProfiles.Where(c => c.ProfileId == FK_TaskMaster).Select(p => p.ProfileName).ToString();
            return TaskDescription;
        }
    }
}
