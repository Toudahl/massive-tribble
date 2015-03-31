using System;
using System.Collections.Generic;
using System.Text;

namespace FetchItUniversalAndApi.Models
{
    public partial class TaskLocationInfoModel
    {
        public int TaskLocationInfoId { get; set; }
        public int FK_TaskId { get; set; }
        public string TaskLocationAddress { get; set; }

        public virtual TaskModel Task { get; set; }
    }
}
