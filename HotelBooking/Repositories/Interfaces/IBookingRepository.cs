using HotelBooking.Models.Entities;
using HotelBooking.Models.Enums;
using HotelBooking.Models.Responses;

namespace HotelBooking.Repositories.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<bool> HasOverlappingConfirmedBooking(int roomId, DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken);
        Task<List<Booking>> GetAllWithDetails(CancellationToken cancellationToken);
        Task<PagedResult<Booking>> GetForUserPaged(
            int userId,
            int? roomId,
            BookingStatus? status,
            string? sortBy,
            bool sortDescending,
            int page,
            int pageSize,
            CancellationToken cancellationToken);
    }
}
