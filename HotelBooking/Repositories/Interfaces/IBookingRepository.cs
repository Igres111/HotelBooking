using HotelBooking.Models.Entities;
using HotelBooking.Models.Enums;
using HotelBooking.Models.Responses;

namespace HotelBooking.Repositories.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<bool> HasOverlappingConfirmedBooking(int roomId, DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken);
        Task<List<Booking>> GetConfirmedBookingsInRange(int roomId, DateTime windowStartUtc, DateTime windowEndUtc, CancellationToken cancellationToken);
        Task<List<Booking>> GetAllWithDetails(CancellationToken cancellationToken);
        Task<List<(BookingStatus Status, int Count)>> GetStatusCounts(DateTime? createdFromUtc, DateTime? createdToUtc, CancellationToken cancellationToken);
        Task<Booking?> GetByIdWithDetails(int id, CancellationToken cancellationToken);
        Task<List<BookingStatusHistory>> GetStatusHistory(int bookingId, CancellationToken cancellationToken);
        Task<PagedResult<Booking>> GetForUserPaged(
            int userId,
            BookingStatus? status,
            bool? isRecurring,
            string? sortBy,
            bool sortDescending,
            int page,
            int pageSize,
            CancellationToken cancellationToken);

        Task<BookingConfirmationResponse> TryConfirm(int bookingId, int actingUserId, CancellationToken cancellationToken);
        Task<BookingRejectionResponse> TryReject(int bookingId, int actingUserId, string? reason, CancellationToken cancellationToken);
        Task<BookingCancellationResponse> TryCancel(int bookingId, int actingUserId, bool isAdmin, string? reason, CancellationToken cancellationToken);
    }
}
