//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FetchItClassLib.Persistence.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class FeedbackModel
    {
        public int FeedbackId { get; set; }
        public int FeedbackRating { get; set; }
        public string FeedbackComment { get; set; }
        public bool FeedbackIsSuspended { get; set; }
        public int FK_FeedbackForTaskId { get; set; }
    
        public virtual TaskModel Task { get; set; }
    }
}
