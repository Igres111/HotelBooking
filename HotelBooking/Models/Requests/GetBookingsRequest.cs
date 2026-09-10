using HotelBooking.Models.Enums;

namespace HotelBooking.Models.Requests
{
    public record GetBookingsRequest(
        int? RoomId = null,
        BookingStatus? Status = null,
        string? SortBy = null,
        bool SortDescending = false,
        int Page = 1,
        int PageSize = 20);
}
