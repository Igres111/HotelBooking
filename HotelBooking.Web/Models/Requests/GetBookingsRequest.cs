using HotelBooking.Web.Models.Enums;

namespace HotelBooking.Web.Models.Requests
{
    public record GetBookingsRequest(
        BookingStatus? Status = null,
        bool? IsRecurring = null,
        string? SortBy = null,
        bool SortDescending = false,
        int Page = 1,
        int PageSize = 20);
}
