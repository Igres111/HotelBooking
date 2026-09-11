namespace HotelBooking.Models.Requests
{
    public record CreateRecurringBookingRequest(
        int RoomId,
        DateOnly StartDate,
        TimeOnly StartTime,
        TimeOnly EndTime,
        int AttendeeCount,
        string? Notes,
        string TimeZoneId,
        int OccurrenceCount);
}
