namespace HotelBooking.Web.Models.Requests
{
    public record CreateMeetingRoomRequest(
        string Name,
        string? Description,
        string Location,
        int Capacity,
        TimeOnly OpeningTime,
        TimeOnly ClosingTime);
}
