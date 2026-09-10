using FluentValidation;
using HotelBooking.Constants;
using HotelBooking.Models.Requests;

namespace HotelBooking.Validators
{
    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator()
        {
            RuleFor(request => request.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(ValidatorConstants.StringLengths.FullNameMaxLength)
                    .WithMessage($"Full name must not exceed {ValidatorConstants.StringLengths.FullNameMaxLength} characters.");

            RuleFor(request => request.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(ValidatorConstants.StringLengths.EmailMaxLength)
                    .WithMessage($"Email must not exceed {ValidatorConstants.StringLengths.EmailMaxLength} characters.");

            RuleFor(request => request.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(ValidatorConstants.StringLengths.PasswordMinLength)
                    .WithMessage($"Password must be at least {ValidatorConstants.StringLengths.PasswordMinLength} characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.");
        }
    }
}
