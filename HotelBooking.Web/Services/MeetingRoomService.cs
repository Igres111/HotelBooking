using HotelBooking.Web.ApiClients.Interfaces;
using HotelBooking.Web.Models.Requests;
using HotelBooking.Web.Models.Responses;
using HotelBooking.Web.Services.Interfaces;

namespace HotelBooking.Web.Services
{
    public class MeetingRoomService(IMeetingRoomApiClient meetingRoomApiClient) : IMeetingRoomService
    {
        public async Task<ResponseWrapper<int>> Create(CreateMeetingRoomRequest request, CancellationToken cancellationToken)
        {
            return await meetingRoomApiClient.Create(request, cancellationToken);
        }

        public async Task<ResponseWrapper<MeetingRoomsResponse>> Update(int id, UpdateMeetingRoomRequest request, CancellationToken cancellationToken)
        {
            return await meetingRoomApiClient.Update(id, request, cancellationToken);
        }

        public async Task<ResponseWrapper<PagedResult<MeetingRoomsResponse>>> GetAllActive(GetMeetingRoomsRequest request, CancellationToken cancellationToken)
        {
            return await meetingRoomApiClient.GetAllActive(request, cancellationToken);
        }

        public async Task<ResponseWrapper<MeetingRoomsResponse>> GetActiveById(int id, CancellationToken cancellationToken)
        {
            return await meetingRoomApiClient.GetActiveById(id, cancellationToken);
        }

        public async Task<ResponseWrapper<List<TimeSlotResponse>>> GetAvailability(int id, GetRoomAvailabilityRequest request, CancellationToken cancellationToken)
        {
            return await meetingRoomApiClient.GetAvailability(id, request, cancellationToken);
        }

        public async Task<ResponseWrapper<List<MeetingRoomsResponse>>> GetAllForAdmin(CancellationToken cancellationToken)
        {
            return await meetingRoomApiClient.GetAllForAdmin(cancellationToken);
        }

        public async Task<ResponseWrapper<MeetingRoomsResponse>> GetByIdForAdmin(int id, CancellationToken cancellationToken)
        {
            return await meetingRoomApiClient.GetByIdForAdmin(id, cancellationToken);
        }
    }
}
