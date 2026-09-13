using HotelBooking.Web.Constants;

namespace HotelBooking.Web.Helpers
{
    public static class BookingValidationHelper
    {
        public static bool IsDurationWithinAllowedRange(TimeOnly startTime, TimeOnly endTime)
        {
            var duration = endTime - startTime;
            return duration >= ValidatorConstants.Numbers.BookingMinimumDuration
                && duration <= ValidatorConstants.Numbers.BookingMaximumDuration;
        }

        public static bool IsThirtyMinuteIncrement(TimeOnly startTime, TimeOnly endTime)
        {
            var duration = endTime - startTime;
            return duration.TotalMinutes % ValidatorConstants.Numbers.BookingMinimumDuration.TotalMinutes == 0;
        }

        public static bool IsValidTimeZone(string timezone)
        {
            return TimeZoneInfo.TryFindSystemTimeZoneById(timezone, out _);
        }

        public static bool IsInTheFuture(DateOnly date, TimeOnly startTime, string timeZoneId)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var startUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(date.ToDateTime(startTime), DateTimeKind.Unspecified), timeZone);
            return startUtc > DateTime.UtcNow;
        }

        public static bool IsWithinAdvanceBookingWindow(DateOnly date, string timeZoneId)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var todayLocal = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone));
            var latestAllowedDate = todayLocal.AddDays(ValidatorConstants.Numbers.MaximumAdvanceBookingDays);
            return date <= latestAllowedDate;
        }
    }
}
