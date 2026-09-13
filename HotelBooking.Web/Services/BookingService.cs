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

        public async Task<ResponseWrapper<List<BookingResponse>>> GetAllForAdmin(CancellationToken cancellationToken)
        {
            return await bookingApiClient.GetAllForAdmin(cancellationToken);
        }

        public async Task<byte[]> ExportFile(CancellationToken cancellationToken)
        {
            return await bookingApiClient.ExportFile(cancellationToken);
        }

        public async Task<ResponseWrapper<BookingResponse>> GetById(int id, CancellationToken cancellationToken)
        {
            return await bookingApiClient.GetById(id, cancellationToken);
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

        public async Task<ResponseWrapper<BookingResponse>> Confirm(int id, CancellationToken cancellationToken)
        {
            return await bookingApiClient.Confirm(id, cancellationToken);
        }

        public async Task<ResponseWrapper<BookingResponse>> Reject(int id, CancellationToken cancellationToken)
        {
            return await bookingApiClient.Reject(id, cancellationToken);
        }
    }
}
