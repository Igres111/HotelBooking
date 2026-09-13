using HotelBooking.Web.ApiClients.Interfaces;
using HotelBooking.Web.Models.Requests;
using HotelBooking.Web.Models.Responses;
using HotelBooking.Web.Models.ViewModels;
using HotelBooking.Web.Services.Interfaces;

namespace HotelBooking.Web.Services
{
    public class DashboardService(
        IDashboardApiClient dashboardApiClient,
        IMeetingRoomService meetingRoomService) : IDashboardService
    {
        public async Task<ResponseWrapper<DashboardResponse>> GetStatistics(GetDashboardRequest request, CancellationToken cancellationToken)
        {
            return await dashboardApiClient.GetStatistics(request, cancellationToken);
        }

        public async Task<DashboardViewModel> GetDashboardViewModel(
            int? year,
            int? month,
            int? roomId,
            DateOnly? date,
            TimeOnly? fromTime,
            TimeOnly? toTime,
            CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            var resolvedYear = year ?? now.Year;
            var resolvedMonth = month ?? now.Month;

            var statisticsResponse = await GetStatistics(new GetDashboardRequest(resolvedYear, resolvedMonth), cancellationToken);

            var roomsResponse = await meetingRoomService.GetAllForAdmin(cancellationToken);
            var activeRooms = (roomsResponse.Data ?? [])
                .Where(room => room.IsActive)
                .OrderBy(room => room.Name)
                .ToList();

            var viewModel = new DashboardViewModel
            {
                Year = resolvedYear,
                Month = resolvedMonth,
                TotalBookings = statisticsResponse.Data?.TotalBookings ?? 0,
                PendingBookings = statisticsResponse.Data?.PendingBookings ?? 0,
                ConfirmedBookings = statisticsResponse.Data?.ConfirmedBookings ?? 0,
                CancelledBookings = statisticsResponse.Data?.CancelledBookings ?? 0,
                Rooms = activeRooms
            };

            var resolvedRoomId = roomId ?? activeRooms.FirstOrDefault()?.Id;
            var selectedRoom = activeRooms.FirstOrDefault(room => room.Id == resolvedRoomId);

            if (selectedRoom is null)
            {
                return viewModel;
            }

            var resolvedDate = date ?? DateOnly.FromDateTime(now);
            var resolvedFromTime = fromTime ?? selectedRoom.OpeningTime;
            var resolvedToTime = toTime ?? selectedRoom.ClosingTime;

            viewModel.RoomId = selectedRoom.Id;
            viewModel.UtilizationDate = resolvedDate;
            viewModel.FromTime = resolvedFromTime;
            viewModel.ToTime = resolvedToTime;

            var occupiedHoursRequest = new GetOccupiedHoursRequest(resolvedDate, resolvedFromTime, resolvedToTime, "Asia/Tbilisi");
            var occupiedHoursResponse = await meetingRoomService.GetOccupiedHours(selectedRoom.Id, occupiedHoursRequest, cancellationToken);

            viewModel.OccupiedSlots = occupiedHoursResponse.Data ?? [];

            return viewModel;
        }
    }
}
