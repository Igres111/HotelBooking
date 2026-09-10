using HotelBooking.Data;
using HotelBooking.Models.Entities;
using HotelBooking.Repositories.BaseRepository;
using HotelBooking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Repositories
{
    public class UserRepository(AppDbContext context) : Repository<User>(context), IUserRepository
    {
        public Task<User?> GetByEmail(string email, CancellationToken cancellationToken)
        {
            return Context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    user =>
                        user.Email == email.ToLower() &&
                        user.DeletedAt == null,
                    cancellationToken);
        }
    }
}
