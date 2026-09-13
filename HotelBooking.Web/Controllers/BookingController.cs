using FluentValidation;
using HotelBooking.Web.Models.Requests;
using HotelBooking.Web.Models.ViewModels;
using HotelBooking.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IValidator<BookTimeSlotViewModel> _bookTimeSlotViewModelValidator;
        private readonly IValidator<CreateRecurringBookingViewModel> _createRecurringBookingViewModelValidator;

        public BookingController(
            IBookingService bookingService,
            IValidator<BookTimeSlotViewModel> bookTimeSlotViewModelValidator,
            IValidator<CreateRecurringBookingViewModel> createRecurringBookingViewModelValidator)
        {
            _bookingService = bookingService;
            _bookTimeSlotViewModelValidator = bookTimeSlotViewModelValidator;
            _createRecurringBookingViewModelValidator = createRecurringBookingViewModelValidator;
        }

        public async Task<IActionResult> Index([FromQuery] GetBookingsRequest request, CancellationToken cancellationToken)
        {
            var nonRecurringRequest = request with { IsRecurring = false };
            var recurringRequest = new GetBookingsRequest(
                Status: request.Status,
                IsRecurring: true,
                SortBy: request.SortBy,
                SortDescending: request.SortDescending,
                Page: 1,
                PageSize: 100);

            var response = await _bookingService.GetAllForUser(nonRecurringRequest, cancellationToken);
            var recurringResponse = await _bookingService.GetAllForUser(recurringRequest, cancellationToken);

            var viewModel = new BookingListViewModel
            {
                Bookings = response.Data,
                RecurringBookings = recurringResponse.Data,
                Status = request.Status,
                SortBy = request.SortBy,
                SortDescending = request.SortDescending
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var response = await _bookingService.GetById(id, cancellationToken);

            if (!response.IsSuccess || response.Data is null)
            {
                return StatusCode(response.StatusCode);
            }

            return View(response.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookTimeSlotViewModel model, CancellationToken cancellationToken)
        {
            var validationResult = await _bookTimeSlotViewModelValidator.ValidateAsync(model, cancellationToken);

            if (!validationResult.IsValid)
            {
                TempData["ErrorMessage"] = string.Join(" ", validationResult.Errors.Select(error => error.ErrorMessage));
                return RedirectToAction("Availability", "MeetingRoom", new
                {
                    id = model.RoomId,
                    date = model.Date,
                    durationMinutes = (int)(model.EndTime - model.StartTime).TotalMinutes,
                    timeZoneId = model.TimeZoneId
                });
            }

            var request = new CreateBookingRequest(model.RoomId, model.Date, model.StartTime, model.EndTime, model.AttendeeCount, model.Notes, model.TimeZoneId);
            var response = await _bookingService.Create(request, cancellationToken);

            TempData[response.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = response.IsSuccess
                ? "Booking request submitted successfully."
                : response.Message;

            return RedirectToAction("Availability", "MeetingRoom", new
            {
                id = model.RoomId,
                date = model.Date,
                durationMinutes = (int)(model.EndTime - model.StartTime).TotalMinutes,
                timeZoneId = model.TimeZoneId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRecurring(CreateRecurringBookingViewModel model, CancellationToken cancellationToken)
        {
            var validationResult = await _createRecurringBookingViewModelValidator.ValidateAsync(model, cancellationToken);

            if (!validationResult.IsValid)
            {
                TempData["ErrorMessage"] = string.Join(" ", validationResult.Errors.Select(error => error.ErrorMessage));
                return RedirectToAction("Availability", "MeetingRoom", new
                {
                    id = model.RoomId,
                    date = model.StartDate,
                    durationMinutes = (int)(model.EndTime - model.StartTime).TotalMinutes,
                    timeZoneId = model.TimeZoneId
                });
            }

            var request = new CreateRecurringBookingRequest(model.RoomId, model.StartDate, model.StartTime, model.EndTime, model.AttendeeCount, model.Notes, model.TimeZoneId, model.OccurrenceCount);
            var response = await _bookingService.CreateRecurring(request, cancellationToken);

            TempData[response.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = response.IsSuccess
                ? "Recurring booking request submitted successfully."
                : response.Message;

            return RedirectToAction("Availability", "MeetingRoom", new
            {
                id = model.RoomId,
                date = model.StartDate,
                durationMinutes = (int)(model.EndTime - model.StartTime).TotalMinutes,
                timeZoneId = model.TimeZoneId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, string? returnUrl, CancellationToken cancellationToken)
        {
            var response = await _bookingService.Cancel(id, cancellationToken);

            TempData[response.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = response.IsSuccess
                ? "Booking cancelled successfully."
                : response.Message;

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id, string? returnUrl, CancellationToken cancellationToken)
        {
            var response = await _bookingService.Confirm(id, cancellationToken);

            TempData[response.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = response.IsSuccess
                ? "Booking confirmed successfully."
                : response.Message;

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("AllBookings", "Admin");
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string? returnUrl, CancellationToken cancellationToken)
        {
            var response = await _bookingService.Reject(id, cancellationToken);

            TempData[response.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = response.IsSuccess
                ? "Booking rejected successfully."
                : response.Message;

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("AllBookings", "Admin");
        }
    }
}
