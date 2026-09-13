using FluentValidation;
using HotelBooking.Web.Constants;
using HotelBooking.Web.Models.ViewModels;

namespace HotelBooking.Web.Validators
{
    public class UpdateMeetingRoomViewModelValidator : AbstractValidator<UpdateMeetingRoomViewModel>
    {
        public UpdateMeetingRoomViewModelValidator()
        {
            RuleFor(model => model.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(ValidatorConstants.StringLengths.RoomNameMaxLength)
                    .WithMessage($"Name must not exceed {ValidatorConstants.StringLengths.RoomNameMaxLength} characters.");

            RuleFor(model => model.Location)
                .NotEmpty().WithMessage("Location is required.")
                .MaximumLength(ValidatorConstants.StringLengths.LocationMaxLength)
                    .WithMessage($"Location must not exceed {ValidatorConstants.StringLengths.LocationMaxLength} characters.");

            RuleFor(model => model.Description)
                .MaximumLength(ValidatorConstants.StringLengths.DescriptionMaxLength)
                    .WithMessage($"Description must not exceed {ValidatorConstants.StringLengths.DescriptionMaxLength} characters.");

            RuleFor(model => model.Capacity)
                .GreaterThan(ValidatorConstants.Numbers.MinimumPositiveValue)
                    .WithMessage("Capacity must be greater than zero.");

            RuleFor(model => model.OpeningTime)
                .LessThan(model => model.ClosingTime)
                .WithMessage("Opening time must be before closing time.");
        }
    }
}
