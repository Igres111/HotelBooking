using HotelBooking.Data;
using HotelBooking.Models.Entities;
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

        public async Task<List<MeetingRoom>> GetAllActiveRooms(CancellationToken cancellationToken)
        {
            return await Context.MeetingRooms
                .Where(room => room.IsActive && room.DeletedAt == null)
                .ToListAsync(cancellationToken);
        }

        public Task<MeetingRoom?> GetActiveById(int id, CancellationToken cancellationToken)
        {
            return Context.MeetingRooms
                .Where(room => room.Id == id && room.IsActive && room.DeletedAt == null)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
