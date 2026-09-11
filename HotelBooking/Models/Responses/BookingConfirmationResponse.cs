using HotelBooking.Models.Entities;
using HotelBooking.Models.Enums;

namespace HotelBooking.Models.Responses
{
    public record BookingConfirmationResponse(BookingConfirmationResult Result, Booking? Booking);
}
