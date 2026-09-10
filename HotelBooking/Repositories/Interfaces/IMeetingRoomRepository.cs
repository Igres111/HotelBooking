using HotelBooking.Models.Entities;

namespace HotelBooking.Repositories.Interfaces
{
    public interface IMeetingRoomRepository : IRepository<MeetingRoom>
    {
        Task<MeetingRoom?> GetByNameAndLocation(string name, string location, CancellationToken cancellationToken);
        Task<List<MeetingRoom>> GetAllActiveRooms(CancellationToken cancellationToken);
        Task<MeetingRoom?> GetActiveById(int id, CancellationToken cancellationToken);
    }
}
