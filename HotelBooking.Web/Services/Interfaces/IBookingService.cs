using HotelBooking.Web.Models.Requests;
using HotelBooking.Web.Models.Responses;

namespace HotelBooking.Web.Services.Interfaces
{
    public interface IBookingService
    {
        Task<ResponseWrapper<PagedResult<BookingResponse>>> GetAllForUser(GetBookingsRequest request, CancellationToken cancellationToken);
        Task<ResponseWrapper<int>> Create(CreateBookingRequest request, CancellationToken cancellationToken);
    }
}
