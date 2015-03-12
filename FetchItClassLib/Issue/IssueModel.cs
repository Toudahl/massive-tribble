using System;
using System.ComponentModel.DataAnnotations;
using FetchItClassLib.Profile;

namespace FetchItClassLib.Issue
{
    public partial class IssueModel
    {
        public int IssueId { get; set; }

        public int IssueCreator { get; set; }

        public int IssueTarget { get; set; }

        [Required]
        [StringLength(15)]
        public string IssueTitle { get; set; }

        [Required]
        public string IssueDescription { get; set; }

        public bool IssueStatus { get; set; }

        [Required]
        public string IssueSolution { get; set; }

        public DateTime IssueDateCreated { get; set; }

        public virtual ProfileModel Profile { get; set; }

        public virtual ProfileModel Profile1 { get; set; }
    }
}
