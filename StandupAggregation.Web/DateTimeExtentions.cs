using System;

namespace StandupAggregation.Web
{
    public static class DateTimeExtentions
    {
        public static string ToEstDateString(this DateTime time)
        {
            DateTime currentTime = TimeZoneInfo.ConvertTime(DateTime.Now,TimeZoneInfo.FindSystemTimeZoneById("US Eastern Standard Time"));
            return currentTime.ToString("yyyy/MM/dd");
        }
    }
}