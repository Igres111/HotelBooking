using HotelBooking.Models.Entities;

namespace HotelBooking.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmail(string email, CancellationToken cancellationToken);
    }
}
