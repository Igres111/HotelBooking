using HotelBooking.Web.Models.Requests;
using HotelBooking.Web.Models.ViewModels;
using HotelBooking.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers
{
    public class MeetingRoomController : Controller
    {
        private readonly IMeetingRoomService _meetingRoomService;

        public MeetingRoomController(IMeetingRoomService meetingRoomService)
        {
            _meetingRoomService = meetingRoomService;
        }

        public async Task<IActionResult> Index([FromQuery] GetMeetingRoomsRequest request, CancellationToken cancellationToken)
        {
            var response = await _meetingRoomService.GetAllActive(request, cancellationToken);

            var viewModel = new RoomCatalogViewModel
            {
                Rooms = response.Data,
                Name = request.Name,
                Location = request.Location,
                MinCapacity = request.MinCapacity,
                SortBy = request.SortBy,
                SortDescending = request.SortDescending
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Availability(int id, DateOnly? date, int? durationMinutes, string? timeZoneId, CancellationToken cancellationToken)
        {
            var roomResponse = await _meetingRoomService.GetActiveById(id, cancellationToken);

            if (!roomResponse.IsSuccess || roomResponse.Data is null)
            {
                return NotFound();
            }

            var effectiveDate = date ?? DateOnly.FromDateTime(DateTime.Now);
            var effectiveDuration = durationMinutes ?? 60;
            var effectiveTimeZoneId = string.IsNullOrWhiteSpace(timeZoneId) ? "Asia/Tbilisi" : timeZoneId;

            var viewModel = new RoomAvailabilityViewModel
            {
                RoomId = id,
                RoomName = roomResponse.Data.Name,
                RoomCapacity = roomResponse.Data.Capacity,
                Date = effectiveDate,
                DurationMinutes = effectiveDuration,
                TimeZoneId = effectiveTimeZoneId
            };

            var availabilityRequest = new GetRoomAvailabilityRequest(effectiveDate, effectiveDuration, effectiveTimeZoneId);
            var availabilityResponse = await _meetingRoomService.GetAvailability(id, availabilityRequest, cancellationToken);

            if (!availabilityResponse.IsSuccess)
            {
                viewModel.ErrorMessage = availabilityResponse.Message;
            }
            else
            {
                viewModel.TimeSlots = availabilityResponse.Data;
            }

            return View(viewModel);
        }
    }
}
