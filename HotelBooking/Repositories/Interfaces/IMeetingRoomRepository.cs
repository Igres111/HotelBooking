using HotelBooking.Models.Entities;
using HotelBooking.Models.Responses;

namespace HotelBooking.Repositories.Interfaces
{
    public interface IMeetingRoomRepository : IRepository<MeetingRoom>
    {
        Task<MeetingRoom?> GetByNameAndLocation(string name, string location, CancellationToken cancellationToken);
        Task<MeetingRoom?> GetActiveById(int id, CancellationToken cancellationToken);

        Task<PagedResult<MeetingRoom>> GetActiveRoomsPaged(
            string? name,
            string? location,
            int? minCapacity,
            string? sortBy,
            bool sortDescending,
            int page,
            int pageSize,
            CancellationToken cancellationToken);
    }
}
