using HotelBooking.Web.Models.Requests;
using HotelBooking.Web.Models.ViewModels;
using HotelBooking.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookTimeSlotViewModel model, CancellationToken cancellationToken)
        {
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
        public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
        {
            var response = await _bookingService.Cancel(id, cancellationToken);

            TempData[response.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = response.IsSuccess
                ? "Booking cancelled successfully."
                : response.Message;

            return RedirectToAction("Index");
        }
    }
}
