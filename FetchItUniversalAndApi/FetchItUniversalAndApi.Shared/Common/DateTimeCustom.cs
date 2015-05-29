using System;

namespace FetchItUniversalAndApi.Common
{
    //Author : Morten Toudahl
    /// <summary>
    /// This class will help convert TimeSpans and DateTimeOffsets to DateTime
    /// </summary>
    static class DateTimeCustom
    {
        /// <summary>
        /// This method will convert a <see cref="DateTimeOffset"/> object and a <see cref="TimeSpan"/> object into a DateTime object
        /// </summary>
        /// <param name="date">The <see cref="DateTimeOffset"/> used for the conversion</param>
        /// <param name="time">The <see cref="TimeSpan"/> used for the conversion</param>
        /// <returns></returns>
        static public DateTime TimeSpanAndDateOffsetToDateTime(DateTimeOffset date, TimeSpan time)
        {
            return new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);
        }
    }
}
