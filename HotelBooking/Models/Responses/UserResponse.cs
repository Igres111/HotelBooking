using HotelBooking.Models.Enums;

namespace HotelBooking.Models.Responses
{
    public record UserResponse(int Id, string Email, UserRole Role);
}
