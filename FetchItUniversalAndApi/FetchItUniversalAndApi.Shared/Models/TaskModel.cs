using System;

namespace FetchItUniversalAndApi.Models
{
    public class TaskModel
    {
        public int TaskId { get; set; }
        public string FK_TaskNotificationReferenceId { get; set; }
        public int FK_TaskMaster { get; set; }
        public int? FK_TaskFetcher { get; set; }
        public string TaskDescription { get; set; }
        public DateTime TaskTimeCreated { get; set; }
        public DateTime TaskDeadline { get; set; }
        public decimal TaskItemPrice { get; set; }
        public decimal TaskFee { get; set; }
        public int FK_TaskStatus { get; set; }
        public string TaskEndPointAddress { get; set; }

        //Author: Lárus Þór Hakarl
        public override string ToString()
        {
            return TaskDescription;
        }
    }
}
