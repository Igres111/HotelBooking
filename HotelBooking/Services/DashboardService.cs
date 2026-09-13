using System.Net;
using FluentValidation;
using HotelBooking.Models.Enums;
using HotelBooking.Models.Requests;
using HotelBooking.Models.Responses;
using HotelBooking.Repositories.Interfaces;
using HotelBooking.Services.Interfaces;

namespace HotelBooking.Services
{
    public class DashboardService(
        IBookingRepository bookingRepository,
        IValidator<GetDashboardRequest> getDashboardRequestValidator) : IDashboardService
    {
        public async Task<ResponseWrapper<DashboardResponse>> GetStatistics(GetDashboardRequest request, CancellationToken cancellationToken)
        {
            await getDashboardRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

            DateTime? createdFromUtc = null;
            DateTime? createdToUtc = null;

            if (request.Month.HasValue)
            {
                var year = request.Year ?? DateTime.UtcNow.Year;
                createdFromUtc = new DateTime(year, request.Month.Value, 1, 0, 0, 0, DateTimeKind.Utc);
                createdToUtc = createdFromUtc.Value.AddMonths(1);
            }
            else if (request.Year.HasValue)
            {
                createdFromUtc = new DateTime(request.Year.Value, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                createdToUtc = createdFromUtc.Value.AddYears(1);
            }

            var statusCounts = await bookingRepository.GetStatusCounts(createdFromUtc, createdToUtc, cancellationToken);

            int CountFor(BookingStatus status)
            {
                var result = statusCounts.FirstOrDefault(count => count.Status == status);

                return result.Count;
            }

            var response = new DashboardResponse(
                TotalBookings: statusCounts.Sum(count => count.Count),
                PendingBookings: CountFor(BookingStatus.Pending),
                ConfirmedBookings: CountFor(BookingStatus.Confirmed),
                CancelledBookings: CountFor(BookingStatus.Cancelled));

            return new ResponseWrapper<DashboardResponse>(
                true,
                (int)HttpStatusCode.OK,
                "Dashboard statistics retrieved successfully.",
                response);
        }
    }
}
