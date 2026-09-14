using FluentValidation;
using FluentValidation.Results;
using HotelBooking.Web.ApiClients.Interfaces;
using HotelBooking.Web.Models.Requests;
using HotelBooking.Web.Models.Responses;
using HotelBooking.Web.Models.ViewModels;
using HotelBooking.Web.Services.Interfaces;

namespace HotelBooking.Web.Services
{
    public class AuthService(
        IAuthApiClient authApiClient,
        IValidator<SignUpViewModel> signUpViewModelValidator,
        IValidator<LoginViewModel> loginViewModelValidator) : IAuthService
    {
        public async Task<ValidationResult> SignUp(SignUpViewModel model, CancellationToken cancellationToken)
        {
            var validationResult = await signUpViewModelValidator.ValidateAsync(model, cancellationToken);

            if (!validationResult.IsValid)
            {
                return validationResult;
            }

            var request = new RegisterUserRequest(model.FullName, model.Email, model.Password);
            var response = await authApiClient.SignUp(request, cancellationToken);

            if (!response.IsSuccess)
            {
                validationResult.Errors.Add(new ValidationFailure(string.Empty, response.Message));
            }

            return validationResult;
        }

        public async Task<LoginResultResponse> Login(LoginViewModel model, CancellationToken cancellationToken)
        {
            var validationResult = await loginViewModelValidator.ValidateAsync(model, cancellationToken);

            if (!validationResult.IsValid)
            {
                return new LoginResultResponse(validationResult, null, null);
            }

            var request = new LoginUserRequest(model.Email, model.Password);
            var result = await authApiClient.Login(request, cancellationToken);

            if (!result.Response.IsSuccess)
            {
                validationResult.Errors.Add(new ValidationFailure(string.Empty, result.Response.Message));
                return new LoginResultResponse(validationResult, null, null);
            }

            return new LoginResultResponse(validationResult, result.Response.Data, result.ApiAuthCookie, result.ApiAntiforgeryCookie, result.ApiAntiforgeryToken);
        }

        public async Task<ResponseWrapper> Logout(CancellationToken cancellationToken)
        {
            return await authApiClient.Logout(cancellationToken);
        }
    }
}
