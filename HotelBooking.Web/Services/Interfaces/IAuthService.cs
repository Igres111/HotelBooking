using FluentValidation.Results;
using HotelBooking.Web.Models.Responses;
using HotelBooking.Web.Models.ViewModels;

namespace HotelBooking.Web.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ValidationResult> SignUp(SignUpViewModel model, CancellationToken cancellationToken);
        Task<LoginResultResponse> Login(LoginViewModel model, CancellationToken cancellationToken);
    }
}
