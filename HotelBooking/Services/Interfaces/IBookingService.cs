using HotelBooking.Models.Requests;
using HotelBooking.Models.Responses;

namespace HotelBooking.Services.Interfaces
{
    public interface IBookingService
    {
        Task<ResponseWrapper<int>> Create(CreateBookingRequest request, int userId, string? idempotencyKey, CancellationToken cancellationToken);
        Task<ResponseWrapper<List<int>>> CreateRecurring(CreateRecurringBookingRequest request, int userId, string? idempotencyKey, CancellationToken cancellationToken);
        Task<ResponseWrapper<List<BookingResponse>>> GetAllForAdmin(CancellationToken cancellationToken);
        Task<ResponseWrapper<PagedResult<BookingResponse>>> GetAllForUser(int userId, GetBookingsRequest request, CancellationToken cancellationToken);
        Task<ResponseWrapper<BookingResponse>> GetById(int bookingId, int actingUserId, bool isAdmin, CancellationToken cancellationToken);
        Task<ResponseWrapper<List<BookingStatusHistoryResponse>>> GetHistory(int bookingId, CancellationToken cancellationToken);
        Task<ResponseWrapper<BookingResponse>> Confirm(int bookingId, int adminUserId, CancellationToken cancellationToken);
        Task<ResponseWrapper<BookingResponse>> Reject(int bookingId, int adminUserId, BookingReasonRequest? request, CancellationToken cancellationToken);
        Task<ResponseWrapper<BookingResponse>> Cancel(int bookingId, int actingUserId, bool isAdmin, BookingReasonRequest? request, CancellationToken cancellationToken);
    }
}
