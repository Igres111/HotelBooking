using HotelBooking.Web.Models.Enums;

namespace HotelBooking.Web.Models.Responses
{
    public record BookingStatusHistoryResponse(
        int Id,
        BookingStatus PreviousStatus,
        BookingStatus NewStatus,
        int ActingUserId,
        string ActingUserFullName,
        string? Reason,
        DateTime ChangedAtUtc);
}
