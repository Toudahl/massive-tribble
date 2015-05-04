using System;
using System.Collections.Generic;
using System.Text;
using FetchItUniversalAndApi.Handlers;

namespace FetchItUniversalAndApi.Models
{
    public partial class IssueStatusModel
    {                

        public IssueStatusModel()
        {
            this.Issues = new HashSet<IssueModel>();
        }

        public int IssueStatusId { get; set; }
        public string IssueStatus { get; set; }

        public virtual ICollection<IssueModel> Issues { get; set; }
    }
}
