namespace SalesPortal.Utilities
{
    public static class TimeConversion
    {
        public static DateTime ConvertToLocal(this DateTime utcDateTime)
        {
            string timeZoneId = TimeZoneInfo.Local.Id;
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            // Convert UTC time to local time
            DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
            return localDateTime;
        }

    }
}
