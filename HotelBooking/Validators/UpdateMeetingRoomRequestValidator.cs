using FluentValidation;
using HotelBooking.Constants;
using HotelBooking.Models.Requests;

namespace HotelBooking.Validators
{
    public class UpdateMeetingRoomRequestValidator : AbstractValidator<UpdateMeetingRoomRequest>
    {
        public UpdateMeetingRoomRequestValidator()
        {
            RuleFor(request => request.Name)
                .NotEmpty().WithMessage("Name cannot be empty.")
                .MaximumLength(ValidatorConstants.StringLengths.RoomNameMaxLength)
                    .WithMessage($"Name must not exceed {ValidatorConstants.StringLengths.RoomNameMaxLength} characters.")
                .When(request => request.Name is not null);

            RuleFor(request => request.Location)
                .NotEmpty().WithMessage("Location cannot be empty.")
                .MaximumLength(ValidatorConstants.StringLengths.LocationMaxLength)
                    .WithMessage($"Location must not exceed {ValidatorConstants.StringLengths.LocationMaxLength} characters.")
                .When(request => request.Location is not null);

            RuleFor(request => request.Description)
                .MaximumLength(ValidatorConstants.StringLengths.DescriptionMaxLength)
                    .WithMessage($"Description must not exceed {ValidatorConstants.StringLengths.DescriptionMaxLength} characters.")
                .When(request => request.Description is not null);

            RuleFor(request => request.Capacity)
                .GreaterThan(ValidatorConstants.Numbers.MinimumPositiveValue)
                    .WithMessage("Capacity must be greater than zero.")
                .When(request => request.Capacity is not null);

            RuleFor(request => request)
                .Must(request => request.OpeningTime!.Value < request.ClosingTime!.Value)
                .WithName("OpeningTime")
                .WithMessage("Opening time must be before closing time.")
                .When(request => request.OpeningTime is not null && request.ClosingTime is not null);
        }
    }
}
