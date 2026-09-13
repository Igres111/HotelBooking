namespace HotelBooking.Models.Requests
{
    public record GetOccupiedHoursRequest(DateOnly Date, TimeOnly FromTime, TimeOnly ToTime, string TimeZoneId);
}
