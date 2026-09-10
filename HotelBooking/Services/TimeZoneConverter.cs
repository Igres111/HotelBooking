using HotelBooking.Services.Interfaces;

namespace HotelBooking.Services
{
    public class TimeZoneConverter : ITimeZoneConverter
    {
        public DateTime ConvertToUtc(DateTime localDateTime, string timeZoneId)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var unspecified = DateTime.SpecifyKind(localDateTime, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(unspecified, timeZone);
        }

        public DateTime ConvertFromUtc(DateTime utcDateTime, string timeZoneId)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var utc = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            return TimeZoneInfo.ConvertTimeFromUtc(utc, timeZone);
        }
    }
}
