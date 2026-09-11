using HotelBooking.Models.Entities;
using HotelBooking.Models.Enums;

namespace HotelBooking.Models.Responses
{
    public record BookingCancellationResponse(BookingCancellationResult Result, Booking? Booking);
}
