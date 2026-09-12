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
            var response = await _bookingService.GetAllForUser(request, cancellationToken);

            var viewModel = new BookingListViewModel
            {
                Bookings = response.Data,
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
            var request = new CreateBookingRequest(model.RoomId, model.Date, model.StartTime, model.EndTime, 1, null, model.TimeZoneId);
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
    }
}
