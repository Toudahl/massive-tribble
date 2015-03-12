using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FetchItClassLib.Comment;
using FetchItClassLib.Feedback;
using FetchItClassLib.Issue;
using FetchItClassLib.Payment;
using FetchItClassLib.Task;

namespace FetchItClassLib.Profile
{
    public partial class ProfileModel
    {
        public ProfileModel()
        {
            Comments = new HashSet<CommentModel>();
            Feedbacks = new HashSet<FeedbackModel>();
            Feedbacks1 = new HashSet<FeedbackModel>();
            Issues = new HashSet<IssueModel>();
            Issues1 = new HashSet<IssueModel>();
            Payments = new HashSet<PaymentModel>();
            Payments1 = new HashSet<PaymentModel>();
            Tasks = new HashSet<TaskModel>();
            Tasks1 = new HashSet<TaskModel>();
        }

        public int ProfileId { get; set; }

        [Required]
        [StringLength(20)]
        public string ProfileName { get; set; }

        [Required]
        [StringLength(50)]
        public string ProfileAddress { get; set; }

        [StringLength(10)]
        public string ProfilePhone { get; set; }

        [StringLength(10)]
        public string ProfileMobile { get; set; }

        [StringLength(50)]
        public string ProfilePassword { get; set; }

        public string ProfileEmail { get; set; }

        public int? ProfileStatus { get; set; }

        public DateTime? ProfileLastLoggedIn { get; set; }

        public int? ProfileLevel { get; set; }

        public bool? ProfileIsVerified { get; set; }

        public virtual ICollection<CommentModel> Comments { get; set; }

        public virtual ICollection<FeedbackModel> Feedbacks { get; set; }

        public virtual ICollection<FeedbackModel> Feedbacks1 { get; set; }

        public virtual ICollection<IssueModel> Issues { get; set; }

        public virtual ICollection<IssueModel> Issues1 { get; set; }

        public virtual ICollection<PaymentModel> Payments { get; set; }

        public virtual ICollection<PaymentModel> Payments1 { get; set; }

        public virtual ProfileLevel ProfileLevel1 { get; set; }

        public virtual ProfileStatus ProfileStatus1 { get; set; }

        public virtual ICollection<TaskModel> Tasks { get; set; }

        public virtual ICollection<TaskModel> Tasks1 { get; set; }
    }
}
