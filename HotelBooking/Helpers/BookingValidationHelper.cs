using HotelBooking.Constants;
using HotelBooking.Services.Interfaces;

namespace HotelBooking.Helpers
{
    public static class BookingValidationHelper
    {
        public static bool IsDurationWithinAllowedRange(TimeSpan duration)
        {
            return duration >= ValidatorConstants.Numbers.BookingMinimumDuration
                && duration <= ValidatorConstants.Numbers.BookingMaximumDuration;
        }

        public static bool IsDurationWithinAllowedRange(TimeOnly startTime, TimeOnly endTime)
        {
            return IsDurationWithinAllowedRange(endTime - startTime);
        }

        public static bool IsThirtyMinuteIncrement(TimeSpan duration)
        {
            return duration.TotalMinutes % ValidatorConstants.Numbers.BookingMinimumDuration.TotalMinutes == 0;
        }

        public static bool IsThirtyMinuteIncrement(TimeOnly startTime, TimeOnly endTime)
        {
            return IsThirtyMinuteIncrement(endTime - startTime);
        }

        public static bool IsValidTimeZone(string timezone)
        {
            return TimeZoneInfo.TryFindSystemTimeZoneById(timezone, out _);
        }

        public static bool IsInTheFuture(DateOnly date, TimeOnly startTime, string timeZoneId, ITimeZoneConverter timeZoneConverter)
        {
            var startUtc = timeZoneConverter.ConvertToUtc(date.ToDateTime(startTime), timeZoneId);
            return startUtc > DateTime.UtcNow;
        }

        public static bool IsWithinAdvanceBookingWindow(DateOnly date, string timeZoneId, ITimeZoneConverter timeZoneConverter)
        {
            var todayLocal = DateOnly.FromDateTime(timeZoneConverter.ConvertFromUtc(DateTime.UtcNow, timeZoneId));
            var latestAllowedDate = todayLocal.AddDays(ValidatorConstants.Numbers.MaximumAdvanceBookingDays);
            return date <= latestAllowedDate;
        }
    }
}
