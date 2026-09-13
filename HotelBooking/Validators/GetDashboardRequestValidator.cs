using FluentValidation;
using HotelBooking.Models.Requests;

namespace HotelBooking.Validators
{
    public class GetDashboardRequestValidator : AbstractValidator<GetDashboardRequest>
    {
        public GetDashboardRequestValidator()
        {
            RuleFor(request => request.Month)
                .InclusiveBetween(1, 12)
                .WithMessage("Month must be between 1 and 12.")
                .When(request => request.Month is not null);
        }
    }
}
