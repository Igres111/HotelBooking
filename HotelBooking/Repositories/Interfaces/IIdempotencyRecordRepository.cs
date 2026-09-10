using HotelBooking.Models.Entities;

namespace HotelBooking.Repositories.Interfaces
{
    public interface IIdempotencyRecordRepository : IRepository<IdempotencyRecord>
    {
        Task<IdempotencyRecord?> GetByKey(int userId, string key, CancellationToken cancellationToken);
    }
}
