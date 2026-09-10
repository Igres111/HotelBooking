namespace HotelBooking.Models.Requests
{
    public record GetMeetingRoomsRequest(
        string? Name = null,
        string? Location = null,
        int? MinCapacity = null,
        string? SortBy = null,
        bool SortDescending = false,
        int Page = 1,
        int PageSize = 20);
}
