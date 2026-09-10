namespace HotelBooking.Models.Requests
{
    public record UpdateMeetingRoomRequest(
        string? Name,
        string? Description,
        string? Location,
        int? Capacity,
        TimeOnly? OpeningTime,
        TimeOnly? ClosingTime,
        bool? IsActive);
}
