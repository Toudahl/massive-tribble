//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Api.EntityFramework
{
    using System.Collections.Generic;
    
    public partial class TaskStatusModel
    {
        public TaskStatusModel()
        {
            this.Tasks = new HashSet<TaskModel>();
        }
    
        public int TaskStatusId { get; set; }
        public string TaskStatus { get; set; }
    
        public virtual ICollection<TaskModel> Tasks { get; set; }
    }
}
