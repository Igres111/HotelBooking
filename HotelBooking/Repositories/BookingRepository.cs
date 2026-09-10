using HotelBooking.Data;
using HotelBooking.Models.Entities;
using HotelBooking.Models.Enums;
using HotelBooking.Models.Responses;
using HotelBooking.Repositories.BaseRepository;
using HotelBooking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Repositories
{
    public class BookingRepository(AppDbContext context) : Repository<Booking>(context), IBookingRepository
    {
        public Task<bool> HasOverlappingConfirmedBooking(int roomId, DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken)
        {
            return Context.Bookings
                .AsNoTracking()
                .AnyAsync(
                    booking =>
                        booking.RoomId == roomId &&
                        booking.Status == BookingStatus.Confirmed &&
                        booking.DeletedAt == null &&
                        booking.StartUtc < endUtc &&
                        startUtc < booking.EndUtc,
                    cancellationToken);
        }

        public Task<List<Booking>> GetAllWithDetails(CancellationToken cancellationToken)
        {
            return Context.Bookings
                .AsNoTracking()
                .Where(booking => booking.DeletedAt == null)
                .Include(booking => booking.Room)
                .Include(booking => booking.User)
                .ToListAsync(cancellationToken);
        }

        public async Task<PagedResult<Booking>> GetForUserPaged(
            int userId,
            int? roomId,
            BookingStatus? status,
            string? sortBy,
            bool sortDescending,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var query = Context.Bookings
                .AsNoTracking()
                .Include(booking => booking.Room)
                .Include(booking => booking.User)
                .Where(booking => booking.UserId == userId && booking.DeletedAt == null);

            if (roomId.HasValue)
            {
                query = query.Where(booking => booking.RoomId == roomId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(booking => booking.Status == status.Value);
            }

            var normalizedSortBy = sortBy?.ToLower();

            if (normalizedSortBy == "status")
            {
                query = sortDescending
                    ? query.OrderByDescending(booking => booking.Status)
                    : query.OrderBy(booking => booking.Status);
            }
            else
            {
                query = sortDescending
                    ? query.OrderByDescending(booking => booking.StartUtc)
                    : query.OrderBy(booking => booking.StartUtc);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Booking>(items, totalCount, page, pageSize);
        }
    }
}
