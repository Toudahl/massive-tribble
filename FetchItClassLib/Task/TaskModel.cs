using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FetchItClassLib.Comment;
using FetchItClassLib.Payment;
using FetchItClassLib.Profile;

namespace FetchItClassLib.Task
{
    public partial class TaskModel
    {
        public TaskModel()
        {
            Comments = new HashSet<CommentModel>();
            Payments = new HashSet<PaymentModel>();
        }

        public int TaskId { get; set; }

        public int TaskMaster { get; set; }

        public int TaskFetcher { get; set; }

        [Required]
        public string TaskDescription { get; set; }

        public DateTime TaskDateCreated { get; set; }

        public DateTime TaskDeadline { get; set; }

        public bool TaskIsActive { get; set; }

        public bool TaskIsSuspended { get; set; }

        public bool TaskIsCompleted { get; set; }

        public virtual ICollection<CommentModel> Comments { get; set; }

        public virtual ICollection<PaymentModel> Payments { get; set; }

        public virtual ProfileModel Profile { get; set; }

        public virtual ProfileModel Profile1 { get; set; }
    }
}
