using System;
using System.Collections.Generic;

namespace FetchItUniversalAndApi.Models
{
    public partial class IssueModel
    {
        public IssueModel()
        {
            this.IssueDetails = new HashSet<IssueDetailModel>();
        }

        public int IssueId { get; set; }
        public string FK_IssueNotificationReferenceId { get; set; }
        public int FK_IssueCreator { get; set; }
        public int FK_IssueTarget { get; set; }
        public string IssueTitle { get; set; }
        public string IssueDescription { get; set; }
        public System.DateTime IssueTimeCreated { get; set; }
        public Nullable<System.DateTime> IssueTimeResolved { get; set; }
        public int FK_IssueStatus { get; set; }
        public bool IssueDidCreatorWin { get; set; }
        public int FK_IssueTask { get; set; }
        public bool IssueAppealedByCreator { get; set; }
        public bool IssueHasBeenAppealed { get; set; }

        public virtual ICollection<IssueDetailModel> IssueDetails { get; set; }
        public virtual IssueStatusModel IssueStatus { get; set; }
        public virtual TaskModel Task { get; set; }
        public virtual ProfileModel IssueCreator { get; set; }
        public virtual ProfileModel IssueTarget { get; set; }
    }
}
