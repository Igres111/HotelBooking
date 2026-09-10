using HotelBooking.Data;
using HotelBooking.Models.Entities;
using HotelBooking.Models.Responses;
using HotelBooking.Repositories.BaseRepository;
using HotelBooking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Repositories
{
    public class MeetingRoomRepository(AppDbContext context) : Repository<MeetingRoom>(context), IMeetingRoomRepository
    {
        public Task<MeetingRoom?> GetByNameAndLocation(string name, string location, CancellationToken cancellationToken)
        {
            return Context.MeetingRooms
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    room =>
                        room.Name == name &&
                        room.Location == location &&
                        room.DeletedAt == null,
                    cancellationToken);
        }

        public Task<MeetingRoom?> GetActiveById(int id, CancellationToken cancellationToken)
        {
            return Context.MeetingRooms
                .AsNoTracking()
                .Where(room => room.Id == id && room.IsActive && room.DeletedAt == null)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<PagedResult<MeetingRoom>> GetActiveRoomsPaged(
            string? name,
            string? location,
            int? minCapacity,
            string? sortBy,
            bool sortDescending,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var query = Context.MeetingRooms
                .AsNoTracking()
                .Where(room => room.IsActive && room.DeletedAt == null);

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(room => room.Name.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                query = query.Where(room => room.Location.Contains(location));
            }

            if (minCapacity.HasValue)
            {
                query = query.Where(room => room.Capacity >= minCapacity.Value);
            }

            var normalizedSortBy = sortBy?.ToLower();

            if (normalizedSortBy == "location")
            {
                query = sortDescending
                    ? query.OrderByDescending(room => room.Location)
                    : query.OrderBy(room => room.Location);
            }
            else if (normalizedSortBy == "capacity")
            {
                query = sortDescending
                    ? query.OrderByDescending(room => room.Capacity)
                    : query.OrderBy(room => room.Capacity);
            }
            else
            {
                query = sortDescending
                    ? query.OrderByDescending(room => room.Name)
                    : query.OrderBy(room => room.Name);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<MeetingRoom>(items, totalCount, page, pageSize);
        }
    }
}
