using System.Security.Claims;
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
        /// Sample request:
        ///
        ///     POST /api/booking
        ///     Idempotency-Key: 3f29c1e2-5e3a-4b7a-9c2d-8a1e6f0b2b31
        ///     {
        ///         "roomId": 1,
        ///         "date": "2026-09-15",
        ///         "startTime": "10:00",
        ///         "endTime": "11:00",
        ///         "attendeeCount": 5,
        ///         "notes": "Team meeting",
        ///         "timeZoneId": "Asia/Tbilisi"
        ///     }
        ///
        /// startTime/endTime are local to timeZoneId - the server converts them to UTC for storage.
        /// The booking is created with Pending status.
        ///
        /// The optional Idempotency-Key header makes retries safe: repeating the same request with the
        /// same key returns the original result instead of creating a duplicate booking, reusing the
        /// key with different booking data is rejected with 409, and concurrent requests carrying the
        /// same key are handled safely - only one can ever create a booking.
        /// </remarks>
        /// <response code="201">Booking created successfully.</response>
        /// <response code="400">Validation failed.</response>
        /// <response code="404">Meeting room not found or is not active.</response>
        /// <response code="409">This room is already booked for the requested time slot, or the idempotency key was reused with different data.</response>
        [HttpPost]
        [Authorize]
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
        /// Retrieves the current user's own bookings, with filtering, sorting, and pagination.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/booking?roomId=1&amp;status=Pending&amp;sortBy=startutc&amp;sortDescending=true&amp;page=1&amp;pageSize=20
        ///
        /// sortBy accepts "startutc" or "status" (defaults to "startutc"). Only bookings belonging
        /// to the current user are returned - see GET /api/booking/admin for the unfiltered admin view.
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
    }
}
