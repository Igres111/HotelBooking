using System.Data;
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

        public Task<List<Booking>> GetConfirmedBookingsInRange(int roomId, DateTime windowStartUtc, DateTime windowEndUtc, CancellationToken cancellationToken)
        {
            return Context.Bookings
                .AsNoTracking()
                .Where(booking =>
                    booking.RoomId == roomId &&
                    booking.Status == BookingStatus.Confirmed &&
                    booking.DeletedAt == null &&
                    booking.StartUtc < windowEndUtc &&
                    windowStartUtc < booking.EndUtc)
                .ToListAsync(cancellationToken);
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

        public Task<Booking?> GetByIdWithDetails(int id, CancellationToken cancellationToken)
        {
            return Context.Bookings
                .AsNoTracking()
                .Include(booking => booking.Room)
                .Include(booking => booking.User)
                .FirstOrDefaultAsync(booking => booking.Id == id && booking.DeletedAt == null, cancellationToken);
        }

        public Task<List<BookingStatusHistory>> GetStatusHistory(int bookingId, CancellationToken cancellationToken)
        {
            return Context.BookingStatusHistories
                .AsNoTracking()
                .Include(history => history.ActingUser)
                .Where(history => history.BookingId == bookingId)
                .OrderBy(history => history.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<PagedResult<Booking>> GetForUserPaged(
            int userId,
            BookingStatus? status,
            bool? isRecurring,
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

            if (status.HasValue)
            {
                query = query.Where(booking => booking.Status == status.Value);
            }

            if (isRecurring.HasValue)
            {
                query = isRecurring.Value
                    ? query.Where(booking => booking.RecurringSeriesId != null)
                    : query.Where(booking => booking.RecurringSeriesId == null);
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

        public async Task<BookingConfirmationResponse> TryConfirm(int bookingId, int actingUserId, CancellationToken cancellationToken)
        {
            await using var transaction = await Context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            var booking = await Context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.DeletedAt == null, cancellationToken);

            if (booking is null)
            {
                return new BookingConfirmationResponse(BookingConfirmationResult.NotFound, null);
            }

            if (booking.Status != BookingStatus.Pending)
            {
                return new BookingConfirmationResponse(BookingConfirmationResult.InvalidTransition, null);
            }

            var hasOverlap = await Context.Bookings
                .AnyAsync(
                    other =>
                        other.Id != booking.Id &&
                        other.RoomId == booking.RoomId &&
                        other.Status == BookingStatus.Confirmed &&
                        other.DeletedAt == null &&
                        other.StartUtc < booking.EndUtc &&
                        booking.StartUtc < other.EndUtc,
                    cancellationToken);

            if (hasOverlap)
            {
                return new BookingConfirmationResponse(BookingConfirmationResult.Overlap, null);
            }

            var history = new BookingStatusHistory
            {
                BookingId = booking.Id,
                PreviousStatus = booking.Status,
                NewStatus = BookingStatus.Confirmed,
                ActingUserId = actingUserId
            };

            booking.Status = BookingStatus.Confirmed;
            booking.UpdatedAt = DateTime.UtcNow;

            await Context.BookingStatusHistories.AddAsync(history, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new BookingConfirmationResponse(BookingConfirmationResult.Confirmed, booking);
        }

        public async Task<BookingRejectionResponse> TryReject(int bookingId, int actingUserId, string? reason, CancellationToken cancellationToken)
        {
            var booking = await Context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.DeletedAt == null, cancellationToken);

            if (booking is null)
            {
                return new BookingRejectionResponse(BookingRejectionResult.NotFound, null);
            }

            if (booking.Status != BookingStatus.Pending)
            {
                return new BookingRejectionResponse(BookingRejectionResult.InvalidTransition, null);
            }

            var history = new BookingStatusHistory
            {
                BookingId = booking.Id,
                PreviousStatus = booking.Status,
                NewStatus = BookingStatus.Rejected,
                ActingUserId = actingUserId,
                Reason = reason
            };

            booking.Status = BookingStatus.Rejected;
            booking.UpdatedAt = DateTime.UtcNow;

            await Context.BookingStatusHistories.AddAsync(history, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);

            return new BookingRejectionResponse(BookingRejectionResult.Rejected, booking);
        }

        public async Task<BookingCancellationResponse> TryCancel(int bookingId, int actingUserId, bool isAdmin, string? reason, CancellationToken cancellationToken)
        {
            var booking = await Context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.DeletedAt == null, cancellationToken);

            if (booking is null)
            {
                return new BookingCancellationResponse(BookingCancellationResult.NotFound, null);
            }

            if (!isAdmin && booking.UserId != actingUserId)
            {
                return new BookingCancellationResponse(BookingCancellationResult.Forbidden, null);
            }

            if (booking.Status != BookingStatus.Pending && booking.Status != BookingStatus.Confirmed)
            {
                return new BookingCancellationResponse(BookingCancellationResult.InvalidTransition, null);
            }

            var history = new BookingStatusHistory
            {
                BookingId = booking.Id,
                PreviousStatus = booking.Status,
                NewStatus = BookingStatus.Cancelled,
                ActingUserId = actingUserId,
                Reason = reason
            };

            booking.Status = BookingStatus.Cancelled;
            booking.UpdatedAt = DateTime.UtcNow;

            await Context.BookingStatusHistories.AddAsync(history, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);

            return new BookingCancellationResponse(BookingCancellationResult.Cancelled, booking);
        }
    }
}
