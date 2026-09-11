using HotelBooking.Models.Entities;
using HotelBooking.Models.Enums;

namespace HotelBooking.Models.Responses
{
    public record BookingRejectionResponse(BookingRejectionResult Result, Booking? Booking);
}
