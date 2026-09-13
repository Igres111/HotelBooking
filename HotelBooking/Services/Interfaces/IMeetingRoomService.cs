using HotelBooking.Models.Requests;
using HotelBooking.Models.Responses;

namespace HotelBooking.Services.Interfaces
{
    public interface IMeetingRoomService
    {
        Task<ResponseWrapper<int>> Create(CreateMeetingRoomRequest request, CancellationToken cancellationToken);
        Task<ResponseWrapper<List<MeetingRoomsResponse>>> GetAllForAdmin(CancellationToken cancellationToken);
        Task<ResponseWrapper<PagedResult<MeetingRoomsResponse>>> GetAllActive(GetMeetingRoomsRequest request, CancellationToken cancellationToken);
        Task<ResponseWrapper<MeetingRoomsResponse>> GetActiveById(int id, CancellationToken cancellationToken);
        Task<ResponseWrapper<MeetingRoomsResponse>> GetByIdForAdmin(int id, CancellationToken cancellationToken);
        Task<ResponseWrapper<MeetingRoomsResponse>> Update(int id, UpdateMeetingRoomRequest request, CancellationToken cancellationToken);
        Task<ResponseWrapper<List<TimeSlotResponse>>> GetAvailability(int roomId, GetRoomAvailabilityRequest request, CancellationToken cancellationToken);
        Task<ResponseWrapper<List<TimeSlotResponse>>> GetOccupiedHours(int roomId, GetOccupiedHoursRequest request, CancellationToken cancellationToken);
    }
}
