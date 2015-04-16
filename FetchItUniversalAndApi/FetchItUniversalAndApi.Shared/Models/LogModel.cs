using System;
using System.Collections.Generic;
using System.Text;

namespace FetchItUniversalAndApi.Models
{
    public partial class LogModel
    {
        public int LogId { get; set; }
        public string LogMessage { get; set; }
        public DateTime LogTime { get; set; }
    }
}
