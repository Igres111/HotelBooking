using FluentValidation;
using HotelBooking.Helpers;
using HotelBooking.Models.Requests;

namespace HotelBooking.Validators
{
    public class GetOccupiedHoursRequestValidator : AbstractValidator<GetOccupiedHoursRequest>
    {
        public GetOccupiedHoursRequestValidator()
        {
            RuleFor(request => request.TimeZoneId)
                .Must(BookingValidationHelper.IsValidTimeZone)
                .WithMessage("Timezone must be a valid IANA timezone identifier.");

            RuleFor(request => request)
                .Must(request => request.FromTime < request.ToTime)
                .WithName("ToTime")
                .WithMessage("'To' time must be after 'from' time.");
        }
    }
}
