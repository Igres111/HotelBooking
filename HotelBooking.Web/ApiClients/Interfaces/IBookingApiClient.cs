using HotelBooking.Web.Models.Requests;
using HotelBooking.Web.Models.Responses;

namespace HotelBooking.Web.ApiClients.Interfaces
{
    public interface IBookingApiClient
    {
        Task<ResponseWrapper<PagedResult<BookingResponse>>> GetAllForUser(GetBookingsRequest request, CancellationToken cancellationToken);
        Task<ResponseWrapper<int>> Create(CreateBookingRequest request, CancellationToken cancellationToken);
    }
}
