using HotelBooking.Models.Enums;

namespace HotelBooking.Models.Responses
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
