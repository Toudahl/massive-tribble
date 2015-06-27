using System;

namespace FetchItUniversalAndApi.Models
{
    public struct IssueModel
    {
        public int IssueId { get; set; }
        public string FK_IssueNotificationReferenceId { get; set; }
        public int FK_IssueCreator { get; set; }
        public int FK_IssueTarget { get; set; }
        public string IssueTitle { get; set; }
        public string IssueDescription { get; set; }
        public DateTime IssueTimeCreated { get; set; }
        public DateTime? IssueTimeResolved { get; set; }
        public int FK_IssueStatus { get; set; }
        public bool IssueDidCreatorWin { get; set; }
        public int FK_IssueTask { get; set; }
        public bool IssueAppealedByCreator { get; set; }
        public bool IssueHasBeenAppealed { get; set; }

    }
}
