﻿using System;

namespace FetchItUniversalAndApi.Models
{
    public struct ReportModel
    {
        public int ReportId { get; set; }
        public int FK_ReportingProfile { get; set; }
        public string ReportMessage { get; set; }
        public DateTime ReportTime { get; set; }
        public int FK_ReportedProfile { get; set; }
    }
}
