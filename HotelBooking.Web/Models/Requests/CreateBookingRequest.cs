namespace HotelBooking.Web.Models.Requests
{
    public record CreateBookingRequest(
        int RoomId,
        DateOnly Date,
        TimeOnly StartTime,
        TimeOnly EndTime,
        int AttendeeCount,
        string? Notes,
        string TimeZoneId);
}
