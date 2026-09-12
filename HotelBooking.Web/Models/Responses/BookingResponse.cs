using HotelBooking.Web.Models.Enums;

namespace HotelBooking.Web.Models.Responses
{
    public record BookingResponse(
        int Id,
        int RoomId,
        string RoomName,
        int UserId,
        string UserFullName,
        DateTime StartUtc,
        DateTime EndUtc,
        int AttendeeCount,
        string? Notes,
        BookingStatus Status,
        int? RecurringSeriesId);
}
