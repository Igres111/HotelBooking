using HotelBooking.Web.Models.Enums;

namespace HotelBooking.Web.Models.Responses
{
    public record UserResponse(int Id, string Email, UserRole Role);
}
