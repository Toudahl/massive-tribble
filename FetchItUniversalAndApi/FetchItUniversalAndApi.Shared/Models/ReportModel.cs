using System;

namespace FetchItUniversalAndApi.Models
{
    public struct ReportModel
    {
        private int _reportId;

        public int ReportId
        {
            get { return _reportId; }
            set
            {
                if (_reportId == 0)
                {
                    _reportId = value;
                }
            }
        }

        public int FK_ReportingProfile { get; set; }
        public string ReportMessage { get; set; }
        public DateTime ReportTime { get; set; }
        public int FK_ReportedProfile { get; set; }
    }
}
