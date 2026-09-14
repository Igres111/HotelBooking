using System.Security.Claims;
using System.Text;
using HotelBooking.Filters;
using HotelBooking.Models.Requests;
using HotelBooking.Models.Responses;
using HotelBooking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Creates a new booking for a meeting room.
        /// </summary>
        /// <remarks>
        /// startTime/endTime are local to timeZoneId - the server converts them to UTC for storage.
        /// The booking is created with Pending status.
        ///
        /// The Idempotency-Key header is required and makes retries safe: repeating the same request
        /// with the same key returns the original result instead of creating a duplicate booking,
        /// reusing the key with different booking data is rejected with 409, and concurrent requests
        /// carrying the same key are handled safely - only one can ever create a booking.
        /// </remarks>
        /// <response code="201">Booking created successfully.</response>
        /// <response code="400">Validation failed, or the Idempotency-Key header is missing.</response>
        /// <response code="404">Meeting room not found or is not active.</response>
        /// <response code="409">This room is already booked for the requested time slot, or the idempotency key was reused with different data.</response>
        [HttpPost]
        [Authorize]
        [ValidateApiAntiforgeryToken]
        [ProducesResponseType(typeof(ResponseWrapper<int>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseWrapper<int>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseWrapper<int>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(
            [FromBody] CreateBookingRequest request,
            [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
            CancellationToken cancellationToken)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var response = await _bookingService.Create(request, userId, idempotencyKey, cancellationToken);

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Creates a weekly recurring booking, atomically.
        /// </summary>
        /// <remarks>
        /// Occurrences repeat weekly starting from startDate, up to 4 total. Every occurrence is
        /// validated independently (future date, business hours, capacity, 30-day advance window,
        /// overlap with a confirmed booking) - if any single occurrence fails, none are created.
        /// All resulting bookings share the same recurring series and start as Pending; an
        /// administrator confirms or rejects each occurrence individually via the normal
        /// confirm/reject endpoints.
        ///
        /// The Idempotency-Key header is required and works the same way as on single booking
        /// creation: repeating the same request with the same key returns the original result
        /// instead of creating duplicate bookings, reusing the key with different data is rejected
        /// with 409, and concurrent requests carrying the same key are handled safely.
        /// </remarks>
        /// <response code="201">Recurring booking created successfully.</response>
        /// <response code="400">Validation failed, or the Idempotency-Key header is missing.</response>
        /// <response code="404">Meeting room not found or is not active.</response>
        /// <response code="409">One of the occurrences is already booked for the requested time slot, or the idempotency key was reused with different data.</response>
        [HttpPost("recurring")]
        [Authorize]
        [ValidateApiAntiforgeryToken]
        [ProducesResponseType(typeof(ResponseWrapper<List<int>>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseWrapper<List<int>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseWrapper<List<int>>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateRecurring(
            [FromBody] CreateRecurringBookingRequest request,
            [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
            CancellationToken cancellationToken)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var response = await _bookingService.CreateRecurring(request, userId, idempotencyKey, cancellationToken);

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Retrieves the current user's own bookings, with filtering, sorting, and pagination.
        /// </summary>
        /// <remarks>
        /// sortBy accepts "startutc" or "status" (defaults to "startutc"). isRecurring filters to
        /// bookings that were created as part of a recurring series (true) or standalone bookings
        /// (false); omit it to return both. Only bookings belonging to the current user are returned -
        /// see GET /api/booking/admin for the unfiltered admin view.
        /// </remarks>
        /// <response code="200">Bookings retrieved successfully.</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ResponseWrapper<PagedResult<BookingResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllForUser([FromQuery] GetBookingsRequest request, CancellationToken cancellationToken)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var response = await _bookingService.GetAllForUser(userId, request, cancellationToken);

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Retrieves a single booking by id.
        /// </summary>
        /// <remarks>
        /// Any authenticated user may call this - an Employee may only view their own booking,
        /// while an Administrator may view any booking.
        /// </remarks>
        /// <response code="200">Booking retrieved successfully.</response>
        /// <response code="403">You do not have permission to view this booking.</response>
        /// <response code="404">Booking not found.</response>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseWrapper<BookingResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<BookingResponse>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseWrapper<BookingResponse>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isAdmin = User.IsInRole("Administrator");

            var response = await _bookingService.GetById(id, userId, isAdmin, cancellationToken);

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Retrieves the full status-change history of a booking.
        /// </summary>
        /// <remarks>
        /// Administrator role required. Entries are ordered chronologically and include the previous
        /// status, the new status, the acting user, the timestamp, and the optional reason recorded
        /// on rejection or cancellation.
        /// </remarks>
        /// <response code="200">Booking history retrieved successfully.</response>
        /// <response code="404">Booking not found.</response>
        [HttpGet("{id}/history")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(ResponseWrapper<List<BookingStatusHistoryResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<List<BookingStatusHistoryResponse>>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHistory([FromRoute] int id, CancellationToken cancellationToken)
        {
            var response = await _bookingService.GetHistory(id, cancellationToken);

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Retrieves every booking, across all users and rooms.
        /// </summary>
        /// <remarks>
        /// Administrator role required.
        /// </remarks>
        /// <response code="200">Bookings retrieved successfully.</response>
        [HttpGet("admin")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(ResponseWrapper<List<BookingResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllForAdmin(CancellationToken cancellationToken)
        {
            var response = await _bookingService.GetAllForAdmin(cancellationToken);

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Exports every booking, across all users and rooms, as a CSV file.
        /// </summary>
        /// <remarks>
        /// Administrator role required. Uses the same underlying data as GET /api/booking/admin.
        /// </remarks>
        /// <response code="200">CSV file generated successfully.</response>
        [HttpGet("admin/export")]
        [Authorize(Roles = "Administrator")]
        [Produces("text/csv")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportFile(CancellationToken cancellationToken)
        {
            var response = await _bookingService.ExportFile(cancellationToken);

            var bytes = Encoding.UTF8.GetBytes(response.Data!);
            var fileName = $"bookings-{DateTime.UtcNow:yyyyMMddHHmmss}.csv";

            return File(bytes, "text/csv", fileName);
        }

        /// <summary>
        /// Confirms a pending booking.
        /// </summary>
        /// <remarks>
        /// Administrator role required. Availability is re-checked at confirmation time under a
        /// database-level lock, so two administrators confirming overlapping bookings at the same
        /// time can never both succeed - the loser gets a 409.
        /// </remarks>
        /// <response code="200">Booking confirmed successfully.</response>
        /// <response code="404">Booking not found.</response>
        /// <response code="409">The booking is not pending, or the room is already booked for that time slot.</response>
        [HttpPost("{id}/confirm")]
        [Authorize(Roles = "Administrator")]
        [ValidateApiAntiforgeryToken]
        [ProducesResponseType(typeof(ResponseWrapper<BookingResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<BookingResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseWrapper<BookingResponse>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Confirm([FromRoute] int id, CancellationToken cancellationToken)
        {
            var adminUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var response = await _bookingService.Confirm(id, adminUserId, cancellationToken);

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Rejects a pending booking.
        /// </summary>
        /// <remarks>
        /// Administrator role required. The request body is optional; an optional reason may be
        /// supplied and is recorded in the booking's status history.
        /// </remarks>
        /// <response code="200">Booking rejected successfully.</response>
        /// <response code="400">Reason exceeds the maximum length.</response>
        /// <response code="404">Booking not found.</response>
        /// <response code="409">Only a pending booking can be rejected.</response>
        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Administrator")]
        [ValidateApiAntiforgeryToken]
        [ProducesResponseType(typeof(ResponseWrapper<BookingResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseWrapper<BookingResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseWrapper<BookingResponse>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Reject([FromRoute] int id, [FromBody] BookingReasonRequest? request, CancellationToken cancellationToken)
        {
            var adminUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var response = await _bookingService.Reject(id, adminUserId, request, cancellationToken);

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Cancels a pending or confirmed booking.
        /// </summary>
        /// <remarks>
        /// Any authenticated user may call this - an Employee may only cancel their own booking,
        /// while an Administrator may cancel any booking. The request body is optional; an optional
        /// reason may be supplied and is recorded in the booking's status history.
        /// </remarks>
        /// <response code="200">Booking cancelled successfully.</response>
        /// <response code="400">Reason exceeds the maximum length.</response>
        /// <response code="403">You do not have permission to cancel this booking.</response>
        /// <response code="404">Booking not found.</response>
        /// <response code="409">Only a pending or confirmed booking can be cancelled.</response>
        [HttpPost("{id}/cancel")]
        [Authorize]
        [ValidateApiAntiforgeryToken]
        [ProducesResponseType(typeof(ResponseWrapper<BookingResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseWrapper<BookingResponse>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseWrapper<BookingResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseWrapper<BookingResponse>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Cancel([FromRoute] int id, [FromBody] BookingReasonRequest? request, CancellationToken cancellationToken)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isAdmin = User.IsInRole("Administrator");

            var response = await _bookingService.Cancel(id, userId, isAdmin, request, cancellationToken);

            return StatusCode(response.StatusCode, response);
        }
    }
}
