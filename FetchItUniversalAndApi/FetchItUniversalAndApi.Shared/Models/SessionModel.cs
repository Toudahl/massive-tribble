using System;

namespace FetchItUniversalAndApi.Models
{
    public partial class SessionModel
    {
        public int SessionId { get; set; }
        public int FK_ProfileId { get; set; }
        public string SessionIp { get; set; }
        public System.DateTime SessionStart { get; set; }
        public Nullable<System.DateTime> SessionEnd { get; set; }

        public virtual ProfileModel Profile { get; set; }
    }
}
