using FluentValidation;
using HotelBooking.Web.Constants;
using HotelBooking.Web.Models.ViewModels;

namespace HotelBooking.Web.Validators
{
    public class SignUpViewModelValidator : AbstractValidator<SignUpViewModel>
    {
        public SignUpViewModelValidator()
        {
            RuleFor(model => model.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(ValidatorConstants.StringLengths.FullNameMaxLength)
                    .WithMessage($"Full name must not exceed {ValidatorConstants.StringLengths.FullNameMaxLength} characters.");

            RuleFor(model => model.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(ValidatorConstants.StringLengths.EmailMaxLength)
                    .WithMessage($"Email must not exceed {ValidatorConstants.StringLengths.EmailMaxLength} characters.");

            RuleFor(model => model.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(ValidatorConstants.StringLengths.PasswordMinLength)
                    .WithMessage($"Password must be at least {ValidatorConstants.StringLengths.PasswordMinLength} characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.");

            RuleFor(model => model.ConfirmPassword)
                .NotEmpty().WithMessage("Please confirm your password.")
                .Equal(model => model.Password).WithMessage("Passwords do not match.");
        }
    }
}
