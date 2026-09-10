using HotelBooking.Models.Enums;

namespace HotelBooking.Models.Responses
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
        BookingStatus Status);
}
