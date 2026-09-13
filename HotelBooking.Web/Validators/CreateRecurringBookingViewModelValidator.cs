using FluentValidation;
using HotelBooking.Web.Constants;
using HotelBooking.Web.Helpers;
using HotelBooking.Web.Models.ViewModels;

namespace HotelBooking.Web.Validators
{
    public class CreateRecurringBookingViewModelValidator : AbstractValidator<CreateRecurringBookingViewModel>
    {
        public CreateRecurringBookingViewModelValidator()
        {
            RuleFor(model => model.RoomId)
                .GreaterThan(ValidatorConstants.Numbers.MinimumPositiveValue)
                    .WithMessage("A valid meeting room must be selected.");

            RuleFor(model => model.AttendeeCount)
                .GreaterThan(ValidatorConstants.Numbers.MinimumPositiveValue)
                    .WithMessage("Attendee count must be greater than zero.");

            RuleFor(model => model.Notes)
                .MaximumLength(ValidatorConstants.StringLengths.NotesMaxLength)
                    .WithMessage($"Notes must not exceed {ValidatorConstants.StringLengths.NotesMaxLength} characters.");

            RuleFor(model => model.TimeZoneId)
                .Must(BookingValidationHelper.IsValidTimeZone)
                    .WithMessage("Timezone must be a valid IANA timezone identifier.");

            RuleFor(model => model.OccurrenceCount)
                .InclusiveBetween(ValidatorConstants.Numbers.MinimumRecurringOccurrences, ValidatorConstants.Numbers.MaximumRecurringOccurrences)
                    .WithMessage($"Occurrence count must be between {ValidatorConstants.Numbers.MinimumRecurringOccurrences} and {ValidatorConstants.Numbers.MaximumRecurringOccurrences}.");

            RuleFor(model => model)
                .Must(model => model.EndTime > model.StartTime)
                .WithName("EndTime")
                .WithMessage("End time must be after start time.");

            RuleFor(model => model)
                .Must(model => BookingValidationHelper.IsDurationWithinAllowedRange(model.StartTime, model.EndTime))
                .WithName("EndTime")
                .WithMessage("Duration must be between 30 minutes and 4 hours.")
                .When(model => model.EndTime > model.StartTime);

            RuleFor(model => model)
                .Must(model => BookingValidationHelper.IsThirtyMinuteIncrement(model.StartTime, model.EndTime))
                .WithName("EndTime")
                .WithMessage("Duration must be in 30-minute increments.")
                .When(model => model.EndTime > model.StartTime);

            RuleFor(model => model)
                .Must(model => BookingValidationHelper.IsInTheFuture(model.StartDate, model.StartTime, model.TimeZoneId))
                .WithName("StartTime")
                .WithMessage("Start time must be in the future.")
                .When(model => BookingValidationHelper.IsValidTimeZone(model.TimeZoneId));
        }
    }
}
