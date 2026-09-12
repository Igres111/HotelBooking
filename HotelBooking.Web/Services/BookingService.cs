using HotelBooking.Web.ApiClients.Interfaces;
using HotelBooking.Web.Models.Requests;
using HotelBooking.Web.Models.Responses;
using HotelBooking.Web.Services.Interfaces;

namespace HotelBooking.Web.Services
{
    public class BookingService(IBookingApiClient bookingApiClient) : IBookingService
    {
        public async Task<ResponseWrapper<PagedResult<BookingResponse>>> GetAllForUser(GetBookingsRequest request, CancellationToken cancellationToken)
        {
            return await bookingApiClient.GetAllForUser(request, cancellationToken);
        }

        public async Task<ResponseWrapper<int>> Create(CreateBookingRequest request, CancellationToken cancellationToken)
        {
            return await bookingApiClient.Create(request, cancellationToken);
        }

        public async Task<ResponseWrapper<List<int>>> CreateRecurring(CreateRecurringBookingRequest request, CancellationToken cancellationToken)
        {
            return await bookingApiClient.CreateRecurring(request, cancellationToken);
        }

        public async Task<ResponseWrapper<BookingResponse>> Cancel(int id, CancellationToken cancellationToken)
        {
            return await bookingApiClient.Cancel(id, cancellationToken);
        }
    }
}
