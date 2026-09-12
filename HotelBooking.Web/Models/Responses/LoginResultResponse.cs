using FluentValidation.Results;

namespace HotelBooking.Web.Models.Responses
{
    public record LoginResultResponse(ValidationResult ValidationResult, UserResponse? User, string? ApiAuthCookie);
}
