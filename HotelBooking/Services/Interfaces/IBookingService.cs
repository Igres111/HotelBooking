using HotelBooking.Models.Requests;
using HotelBooking.Models.Responses;

namespace HotelBooking.Services.Interfaces
{
    public interface IBookingService
    {
        Task<ResponseWrapper<int>> Create(CreateBookingRequest request, int userId, string? idempotencyKey, CancellationToken cancellationToken);
        Task<ResponseWrapper<List<BookingResponse>>> GetAllForAdmin(CancellationToken cancellationToken);
        Task<ResponseWrapper<PagedResult<BookingResponse>>> GetAllForUser(int userId, GetBookingsRequest request, CancellationToken cancellationToken);
    }
}
