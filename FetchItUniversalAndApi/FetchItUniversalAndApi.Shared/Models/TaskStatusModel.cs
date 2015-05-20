using System.Collections.Generic;

namespace FetchItUniversalAndApi.Models
{
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
