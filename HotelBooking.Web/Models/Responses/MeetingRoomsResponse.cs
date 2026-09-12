namespace HotelBooking.Web.Models.Responses
{
    public record MeetingRoomsResponse(
        int Id,
        string Name,
        string? Description,
        string Location,
        int Capacity,
        TimeOnly OpeningTime,
        TimeOnly ClosingTime,
        bool IsActive);
}
