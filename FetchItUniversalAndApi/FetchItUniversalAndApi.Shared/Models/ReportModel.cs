namespace FetchItUniversalAndApi.Models
{
    public partial class ReportModel
    {
        public int ReportId { get; set; }
        public int FK_ReportingProfile { get; set; }
        public string ReportMessage { get; set; }
        public System.DateTime ReportTime { get; set; }
        public int FK_ReportedProfile { get; set; }

        public virtual ProfileModel ReportedProfile { get; set; }
        public virtual ProfileModel ReportingProfile { get; set; }
    }
}
