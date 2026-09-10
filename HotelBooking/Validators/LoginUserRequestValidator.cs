using FluentValidation;
using HotelBooking.Constants;
using HotelBooking.Models.Requests;

namespace HotelBooking.Validators
{
    public class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
    {
        public LoginUserRequestValidator()
        {
            RuleFor(request => request.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(ValidatorConstants.StringLengths.EmailMaxLength)
                    .WithMessage($"Email must not exceed {ValidatorConstants.StringLengths.EmailMaxLength} characters.");

            RuleFor(request => request.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MaximumLength(ValidatorConstants.StringLengths.PasswordMaxLength)
                    .WithMessage($"Password must not exceed {ValidatorConstants.StringLengths.PasswordMaxLength} characters.");
        }
    }
}
