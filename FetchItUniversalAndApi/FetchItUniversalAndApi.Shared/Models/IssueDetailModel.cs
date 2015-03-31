using System;
using System.Collections.Generic;
using System.Text;

namespace FetchItUniversalAndApi.Models
{
    public partial class IssueDetailModel
    {
        public int IssueDetailId { get; set; }
        public int FK_Issue { get; set; }
        public bool IssueDetailFromCreator { get; set; }
        public byte[] IssueImage { get; set; }
        public string IssueDetailText { get; set; }

        public virtual IssueModel Issue { get; set; }
    }
}
