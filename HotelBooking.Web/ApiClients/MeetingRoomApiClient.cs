using System.Net.Http.Json;
using HotelBooking.Web.ApiClients.Interfaces;
using HotelBooking.Web.Helpers;
using HotelBooking.Web.Models.Requests;
using HotelBooking.Web.Models.Responses;
using Microsoft.AspNetCore.WebUtilities;

namespace HotelBooking.Web.ApiClients
{
    public class MeetingRoomApiClient(HttpClient httpClient) : IMeetingRoomApiClient
    {
        public async Task<ResponseWrapper<int>> Create(CreateMeetingRoomRequest request, CancellationToken cancellationToken)
        {
            var response = await httpClient.PostAsJsonAsync("api/meetingroom", request, cancellationToken);

            return await ApiResponseHelper.ParseResponse<int>(response, cancellationToken);
        }

        public async Task<ResponseWrapper<MeetingRoomsResponse>> Update(int id, UpdateMeetingRoomRequest request, CancellationToken cancellationToken)
        {
            var response = await httpClient.PutAsJsonAsync($"api/meetingroom/{id}", request, cancellationToken);

            return await ApiResponseHelper.ParseResponse<MeetingRoomsResponse>(response, cancellationToken);
        }

        public async Task<ResponseWrapper<PagedResult<MeetingRoomsResponse>>> GetAllActive(GetMeetingRoomsRequest request, CancellationToken cancellationToken)
        {
            var queryParams = new Dictionary<string, string?>
            {
                ["Page"] = request.Page.ToString(),
                ["PageSize"] = request.PageSize.ToString(),
                ["SortDescending"] = request.SortDescending.ToString()
            };

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                queryParams["Name"] = request.Name;
            }

            if (!string.IsNullOrWhiteSpace(request.Location))
            {
                queryParams["Location"] = request.Location;
            }

            if (request.MinCapacity.HasValue)
            {
                queryParams["MinCapacity"] = request.MinCapacity.Value.ToString();
            }

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                queryParams["SortBy"] = request.SortBy;
            }

            var url = QueryHelpers.AddQueryString("api/meetingroom", queryParams);

            var response = await httpClient.GetAsync(url, cancellationToken);

            return await ApiResponseHelper.ParseResponse<PagedResult<MeetingRoomsResponse>>(response, cancellationToken);
        }

        public async Task<ResponseWrapper<MeetingRoomsResponse>> GetActiveById(int id, CancellationToken cancellationToken)
        {
            var response = await httpClient.GetAsync($"api/meetingroom/{id}", cancellationToken);

            return await ApiResponseHelper.ParseResponse<MeetingRoomsResponse>(response, cancellationToken);
        }

        public async Task<ResponseWrapper<List<TimeSlotResponse>>> GetAvailability(int id, GetRoomAvailabilityRequest request, CancellationToken cancellationToken)
        {
            var queryParams = new Dictionary<string, string?>
            {
                ["Date"] = request.Date.ToString("yyyy-MM-dd"),
                ["DurationMinutes"] = request.DurationMinutes.ToString(),
                ["TimeZoneId"] = request.TimeZoneId
            };

            var url = QueryHelpers.AddQueryString($"api/meetingroom/{id}/availability", queryParams);

            var response = await httpClient.GetAsync(url, cancellationToken);

            return await ApiResponseHelper.ParseResponse<List<TimeSlotResponse>>(response, cancellationToken);
        }

        public async Task<ResponseWrapper<List<TimeSlotResponse>>> GetOccupiedHours(int id, GetOccupiedHoursRequest request, CancellationToken cancellationToken)
        {
            var queryParams = new Dictionary<string, string?>
            {
                ["Date"] = request.Date.ToString("yyyy-MM-dd"),
                ["FromTime"] = request.FromTime.ToString("HH:mm"),
                ["ToTime"] = request.ToTime.ToString("HH:mm"),
                ["TimeZoneId"] = request.TimeZoneId
            };

            var url = QueryHelpers.AddQueryString($"api/meetingroom/{id}/occupied-hours", queryParams);

            var response = await httpClient.GetAsync(url, cancellationToken);

            return await ApiResponseHelper.ParseResponse<List<TimeSlotResponse>>(response, cancellationToken);
        }

        public async Task<ResponseWrapper<List<MeetingRoomsResponse>>> GetAllForAdmin(CancellationToken cancellationToken)
        {
            var response = await httpClient.GetAsync("api/meetingroom/admin", cancellationToken);

            return await ApiResponseHelper.ParseResponse<List<MeetingRoomsResponse>>(response, cancellationToken);
        }

        public async Task<ResponseWrapper<MeetingRoomsResponse>> GetByIdForAdmin(int id, CancellationToken cancellationToken)
        {
            var response = await httpClient.GetAsync($"api/meetingroom/admin/{id}", cancellationToken);

            return await ApiResponseHelper.ParseResponse<MeetingRoomsResponse>(response, cancellationToken);
        }
    }
}
