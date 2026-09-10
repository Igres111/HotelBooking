using HotelBooking.Constants;
using HotelBooking.Models.Requests;
using HotelBooking.Services.Interfaces;

namespace HotelBooking.Helpers
{
    public static class BookingValidationHelper
    {
        public static bool IsDurationWithinAllowedRange(CreateBookingRequest request)
        {
            var duration = request.EndTime - request.StartTime;
            return duration >= ValidatorConstants.Numbers.BookingMinimumDuration
                && duration <= ValidatorConstants.Numbers.BookingMaximumDuration;
        }

        public static bool IsThirtyMinuteIncrement(CreateBookingRequest request)
        {
            var duration = request.EndTime - request.StartTime;
            return duration.TotalMinutes % ValidatorConstants.Numbers.BookingMinimumDuration.TotalMinutes == 0;
        }

        public static bool IsValidTimeZone(string timezone)
        {
            return TimeZoneInfo.TryFindSystemTimeZoneById(timezone, out _);
        }

        public static bool IsInTheFuture(CreateBookingRequest request, ITimeZoneConverter timeZoneConverter)
        {
            var startUtc = timeZoneConverter.ConvertToUtc(request.Date.ToDateTime(request.StartTime), request.TimeZoneId);
            return startUtc > DateTime.UtcNow;
        }

        public static bool IsWithinAdvanceBookingWindow(CreateBookingRequest request, ITimeZoneConverter timeZoneConverter)
        {
            var todayLocal = DateOnly.FromDateTime(timeZoneConverter.ConvertFromUtc(DateTime.UtcNow, request.TimeZoneId));
            var latestAllowedDate = todayLocal.AddDays(ValidatorConstants.Numbers.MaximumAdvanceBookingDays);
            return request.Date <= latestAllowedDate;
        }
    }
}
