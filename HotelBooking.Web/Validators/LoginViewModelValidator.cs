using FluentValidation;
using HotelBooking.Web.Constants;
using HotelBooking.Web.Models.ViewModels;

namespace HotelBooking.Web.Validators
{
    public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
    {
        public LoginViewModelValidator()
        {
            RuleFor(model => model.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(ValidatorConstants.StringLengths.EmailMaxLength)
                    .WithMessage($"Email must not exceed {ValidatorConstants.StringLengths.EmailMaxLength} characters.");

            RuleFor(model => model.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MaximumLength(ValidatorConstants.StringLengths.PasswordMaxLength)
                    .WithMessage($"Password must not exceed {ValidatorConstants.StringLengths.PasswordMaxLength} characters.");
        }
    }
}
