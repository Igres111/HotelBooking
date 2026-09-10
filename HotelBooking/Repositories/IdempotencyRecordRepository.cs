using HotelBooking.Data;
using HotelBooking.Models.Entities;
using HotelBooking.Repositories.BaseRepository;
using HotelBooking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Repositories
{
    public class IdempotencyRecordRepository(AppDbContext context) : Repository<IdempotencyRecord>(context), IIdempotencyRecordRepository
    {
        public Task<IdempotencyRecord?> GetByKey(int userId, string key, CancellationToken cancellationToken)
        {
            return Context.IdempotencyRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    record =>
                        record.UserId == userId &&
                        record.Key == key &&
                        record.DeletedAt == null,
                    cancellationToken);
        }
    }
}
