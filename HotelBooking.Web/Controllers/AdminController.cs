using FluentValidation;
using HotelBooking.Web.Models.ViewModels;
using HotelBooking.Web.Models.Requests;
using HotelBooking.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly IMeetingRoomService _meetingRoomService;
        private readonly IBookingService _bookingService;
        private readonly IValidator<CreateMeetingRoomViewModel> _createMeetingRoomViewModelValidator;
        private readonly IValidator<UpdateMeetingRoomViewModel> _updateMeetingRoomViewModelValidator;

        public AdminController(
            IMeetingRoomService meetingRoomService,
            IBookingService bookingService,
            IValidator<CreateMeetingRoomViewModel> createMeetingRoomViewModelValidator,
            IValidator<UpdateMeetingRoomViewModel> updateMeetingRoomViewModelValidator)
        {
            _meetingRoomService = meetingRoomService;
            _bookingService = bookingService;
            _createMeetingRoomViewModelValidator = createMeetingRoomViewModelValidator;
            _updateMeetingRoomViewModelValidator = updateMeetingRoomViewModelValidator;
        }

        public async Task<IActionResult> AllBookings(CancellationToken cancellationToken)
        {
            var response = await _bookingService.GetAllForAdmin(cancellationToken);

            return View(response.Data ?? []);
        }

        public async Task<IActionResult> AllRooms(CancellationToken cancellationToken)
        {
            var response = await _meetingRoomService.GetAllForAdmin(cancellationToken);

            return View(response.Data ?? []);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoom(CreateMeetingRoomViewModel model, CancellationToken cancellationToken)
        {
            var validationResult = await _createMeetingRoomViewModelValidator.ValidateAsync(model, cancellationToken);

            if (!validationResult.IsValid)
            {
                TempData["ErrorMessage"] = string.Join(" ", validationResult.Errors.Select(error => error.ErrorMessage));
                return RedirectToAction("AllRooms");
            }

            var request = new CreateMeetingRoomRequest(model.Name, model.Description, model.Location, model.Capacity, model.OpeningTime, model.ClosingTime);
            var response = await _meetingRoomService.Create(request, cancellationToken);

            TempData[response.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = response.IsSuccess
                ? "Room created successfully."
                : response.Message;

            return RedirectToAction("AllRooms");
        }

        public async Task<IActionResult> RoomDetails(int id, CancellationToken cancellationToken)
        {
            var response = await _meetingRoomService.GetByIdForAdmin(id, cancellationToken);

            if (!response.IsSuccess || response.Data is null)
            {
                return StatusCode(response.StatusCode);
            }

            return View(response.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRoom(int id, UpdateMeetingRoomViewModel model, CancellationToken cancellationToken)
        {
            var validationResult = await _updateMeetingRoomViewModelValidator.ValidateAsync(model, cancellationToken);

            if (!validationResult.IsValid)
            {
                TempData["ErrorMessage"] = string.Join(" ", validationResult.Errors.Select(error => error.ErrorMessage));
                return RedirectToAction("RoomDetails", new { id });
            }

            var request = new UpdateMeetingRoomRequest(model.Name, model.Description, model.Location, model.Capacity, model.OpeningTime, model.ClosingTime, model.IsActive);
            var response = await _meetingRoomService.Update(id, request, cancellationToken);

            TempData[response.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = response.IsSuccess
                ? "Room updated successfully."
                : response.Message;

            return RedirectToAction("RoomDetails", new { id });
        }
    }
}
