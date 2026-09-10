using HotelBooking.Data;
using HotelBooking.Models.BaseTypes;
using HotelBooking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Repositories.BaseRepository
{
    public class Repository<T>(AppDbContext context) : IRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext Context = context;

        public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await Context.Set<T>()
                .Where(entity => EF.Property<int>(entity, "Id") == id && entity.DeletedAt == null)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken)
        {
            await Context.Set<T>().AddAsync(entity, cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Context.SaveChangesAsync(cancellationToken);
        }
    }
}
