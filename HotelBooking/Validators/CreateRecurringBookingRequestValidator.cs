using FluentValidation;
using HotelBooking.Constants;
using HotelBooking.Helpers;
using HotelBooking.Models.Requests;
using HotelBooking.Services.Interfaces;

namespace HotelBooking.Validators
{
    public class CreateRecurringBookingRequestValidator : AbstractValidator<CreateRecurringBookingRequest>
    {
        public CreateRecurringBookingRequestValidator(ITimeZoneConverter timeZoneConverter)
        {
            RuleFor(request => request.RoomId)
                .GreaterThan(ValidatorConstants.Numbers.MinimumPositiveValue)
                    .WithMessage("A valid meeting room must be selected.");

            RuleFor(request => request.AttendeeCount)
                .GreaterThan(ValidatorConstants.Numbers.MinimumPositiveValue)
                    .WithMessage("Attendee count must be greater than zero.");

            RuleFor(request => request.Notes)
                .MaximumLength(ValidatorConstants.StringLengths.NotesMaxLength)
                    .WithMessage($"Notes must not exceed {ValidatorConstants.StringLengths.NotesMaxLength} characters.");

            RuleFor(request => request.TimeZoneId)
                .Must(BookingValidationHelper.IsValidTimeZone)
                .WithMessage("Timezone must be a valid IANA timezone identifier.");

            RuleFor(request => request.OccurrenceCount)
                .InclusiveBetween(ValidatorConstants.Numbers.MinimumRecurringOccurrences, ValidatorConstants.Numbers.MaximumRecurringOccurrences)
                .WithMessage($"Occurrence count must be between {ValidatorConstants.Numbers.MinimumRecurringOccurrences} and {ValidatorConstants.Numbers.MaximumRecurringOccurrences}.");

            RuleFor(request => request)
                .Must(request => request.EndTime > request.StartTime)
                .WithName("EndTime")
                .WithMessage("End time must be after start time.");

            RuleFor(request => request)
                .Must(request => BookingValidationHelper.IsDurationWithinAllowedRange(request.StartTime, request.EndTime))
                .WithName("EndTime")
                .WithMessage("Duration must be between 30 minutes and 4 hours.")
                .When(request => request.EndTime > request.StartTime);

            RuleFor(request => request)
                .Must(request => BookingValidationHelper.IsThirtyMinuteIncrement(request.StartTime, request.EndTime))
                .WithName("EndTime")
                .WithMessage("Duration must be in 30-minute increments.")
                .When(request => request.EndTime > request.StartTime);

            RuleFor(request => request)
                .Must(request => BookingValidationHelper.IsInTheFuture(request.StartDate, request.StartTime, request.TimeZoneId, timeZoneConverter))
                .WithName("StartTime")
                .WithMessage("Start time must be in the future.")
                .When(request => BookingValidationHelper.IsValidTimeZone(request.TimeZoneId));
        }
    }
}
