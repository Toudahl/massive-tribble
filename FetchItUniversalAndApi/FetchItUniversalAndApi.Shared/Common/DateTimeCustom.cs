using System;

namespace FetchItUniversalAndApi.Common
{
    static class DateTimeCustom
    {
        static public DateTime TimeSpanAndDateOffsetToDateTime(DateTimeOffset date, TimeSpan time)
        {
            return new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);
        }
    }
}
