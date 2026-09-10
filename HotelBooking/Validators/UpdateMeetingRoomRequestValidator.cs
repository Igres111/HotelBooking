using FluentValidation;
using HotelBooking.Models.Requests;

namespace HotelBooking.Validators
{
    public class UpdateMeetingRoomRequestValidator : AbstractValidator<UpdateMeetingRoomRequest>
    {
        public UpdateMeetingRoomRequestValidator()
        {
            RuleFor(request => request.Name)
                .NotEmpty().WithMessage("Name cannot be empty.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.")
                .When(request => request.Name is not null);

            RuleFor(request => request.Location)
                .NotEmpty().WithMessage("Location cannot be empty.")
                .MaximumLength(200).WithMessage("Location must not exceed 200 characters.")
                .When(request => request.Location is not null);

            RuleFor(request => request.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
                .When(request => request.Description is not null);

            RuleFor(request => request.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than zero.")
                .When(request => request.Capacity is not null);

            RuleFor(request => request)
                .Must(request => request.OpeningTime!.Value < request.ClosingTime!.Value)
                .WithName("OpeningTime")
                .WithMessage("Opening time must be before closing time.")
                .When(request => request.OpeningTime is not null && request.ClosingTime is not null);
        }
    }
}
