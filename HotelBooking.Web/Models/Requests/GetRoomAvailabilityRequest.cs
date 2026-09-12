namespace HotelBooking.Web.Models.Requests
{
    public record GetRoomAvailabilityRequest(DateOnly Date, int DurationMinutes, string TimeZoneId);
}
