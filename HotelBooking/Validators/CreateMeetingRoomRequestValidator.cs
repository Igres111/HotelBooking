using FluentValidation;
using HotelBooking.Constants;
using HotelBooking.Models.Requests;

namespace HotelBooking.Validators
{
    public class CreateMeetingRoomRequestValidator : AbstractValidator<CreateMeetingRoomRequest>
    {
        public CreateMeetingRoomRequestValidator()
        {
            RuleFor(request => request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(ValidatorConstants.StringLengths.RoomNameMaxLength)
                    .WithMessage($"Name must not exceed {ValidatorConstants.StringLengths.RoomNameMaxLength} characters.");

            RuleFor(request => request.Location)
                .NotEmpty().WithMessage("Location is required.")
                .MaximumLength(ValidatorConstants.StringLengths.LocationMaxLength)
                    .WithMessage($"Location must not exceed {ValidatorConstants.StringLengths.LocationMaxLength} characters.");

            RuleFor(request => request.Description)
                .MaximumLength(ValidatorConstants.StringLengths.DescriptionMaxLength)
                    .WithMessage($"Description must not exceed {ValidatorConstants.StringLengths.DescriptionMaxLength} characters.");

            RuleFor(request => request.Capacity)
                .GreaterThan(ValidatorConstants.Numbers.MinimumPositiveValue)
                    .WithMessage("Capacity must be greater than zero.");

            RuleFor(request => request.OpeningTime)
                .LessThan(request => request.ClosingTime)
                .WithMessage("Opening time must be before closing time.");
        }
    }
}
