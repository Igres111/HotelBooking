using FluentValidation;
using HotelBooking.Models.Requests;

namespace HotelBooking.Validators
{
    public class CreateMeetingRoomRequestValidator : AbstractValidator<CreateMeetingRoomRequest>
    {
        public CreateMeetingRoomRequestValidator()
        {
            RuleFor(request => request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(request => request.Location)
                .NotEmpty().WithMessage("Location is required.")
                .MaximumLength(200).WithMessage("Location must not exceed 200 characters.");

            RuleFor(request => request.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

            RuleFor(request => request.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than zero.");

            RuleFor(request => request.OpeningTime)
                .LessThan(request => request.ClosingTime)
                .WithMessage("Opening time must be before closing time.");
        }
    }
}
