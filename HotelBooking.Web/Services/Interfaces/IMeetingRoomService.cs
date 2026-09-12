using HotelBooking.Web.Models.Requests;
using HotelBooking.Web.Models.Responses;

namespace HotelBooking.Web.Services.Interfaces
{
    public interface IMeetingRoomService
    {
        Task<ResponseWrapper<PagedResult<MeetingRoomsResponse>>> GetAllActive(GetMeetingRoomsRequest request, CancellationToken cancellationToken);
        Task<ResponseWrapper<MeetingRoomsResponse>> GetActiveById(int id, CancellationToken cancellationToken);
        Task<ResponseWrapper<List<TimeSlotResponse>>> GetAvailability(int id, GetRoomAvailabilityRequest request, CancellationToken cancellationToken);
    }
}
