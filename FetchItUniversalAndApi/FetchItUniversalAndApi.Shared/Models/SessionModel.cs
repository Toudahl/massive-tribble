using System;

namespace FetchItUniversalAndApi.Models
{
    public class SessionModel
    {
        public int SessionId { get; set; }
        public int FK_ProfileId { get; set; }
        public string SessionIp { get; set; }
        public DateTime SessionStart { get; set; }
        public DateTime? SessionEnd { get; set; }
    }
}
