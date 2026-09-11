using FluentValidation;
using HotelBooking.Helpers;
using HotelBooking.Models.Requests;

namespace HotelBooking.Validators
{
    public class GetRoomAvailabilityRequestValidator : AbstractValidator<GetRoomAvailabilityRequest>
    {
        public GetRoomAvailabilityRequestValidator()
        {
            RuleFor(request => request.TimeZoneId)
                .Must(BookingValidationHelper.IsValidTimeZone)
                .WithMessage("Timezone must be a valid IANA timezone identifier.");

            RuleFor(request => request.DurationMinutes)
                .Must(durationMinutes => BookingValidationHelper.IsDurationWithinAllowedRange(TimeSpan.FromMinutes(durationMinutes)))
                .WithMessage("Duration must be between 30 minutes and 4 hours.");

            RuleFor(request => request.DurationMinutes)
                .Must(durationMinutes => BookingValidationHelper.IsThirtyMinuteIncrement(TimeSpan.FromMinutes(durationMinutes)))
                .WithMessage("Duration must be in 30-minute increments.");
        }
    }
}
