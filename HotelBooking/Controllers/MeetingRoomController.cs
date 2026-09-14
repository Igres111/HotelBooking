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
    public class MeetingRoomController : ControllerBase
    {
        private readonly IMeetingRoomService _meetingRoomService;

        public MeetingRoomController(IMeetingRoomService meetingRoomService)
        {
            _meetingRoomService = meetingRoomService;
        }

        /// <summary>
        /// Creates a new meeting room.
        /// </summary>
        /// <remarks>
        /// Administrator role required.
        /// </remarks>
        /// <response code="201">Meeting room created successfully.</response>
        /// <response code="400">Validation failed.</response>
        /// <response code="409">A meeting room with this name already exists at this location.</response>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateApiAntiforgeryToken]
        [ProducesResponseType(typeof(ResponseWrapper<int>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseWrapper<int>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateMeetingRoomRequest request, CancellationToken cancellationToken)
        {
            var response = await _meetingRoomService.Create(request, cancellationToken);

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Retrieves active meeting rooms, with search, filtering, sorting, and pagination.
        /// </summary>
        /// <remarks>
        /// Excludes deactivated rooms - see GET /api/meetingroom/admin for those. sortBy accepts
        /// "name", "location", or "capacity" (defaults to "name").
        /// </remarks>
        /// <response code="200">Meeting rooms retrieved successfully.</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ResponseWrapper<PagedResult<MeetingRoomsResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllActive([FromQuery] GetMeetingRoomsRequest request, CancellationToken cancellationToken)
        {
            var response = await _meetingRoomService.GetAllActive(request, cancellationToken);

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Retrieves a single meeting room by id.
        /// </summary>
        /// <response code="200">Meeting room retrieved successfully.</response>
        /// <response code="404">Meeting room not found.</response>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseWrapper<MeetingRoomsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<MeetingRoomsResponse>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetActiveById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var response = await _meetingRoomService.GetActiveById(id, cancellationToken);

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Retrieves the available time slots for a room on a given date, for a requested duration.
        /// </summary>
        /// <remarks>
        /// date and returned slot times are local to timeZoneId. Only slots within business hours
        /// that don't overlap a Confirmed booking are returned - Pending, Rejected, and Cancelled
        /// bookings never block availability, and back-to-back slots are allowed.
        /// </remarks>
        /// <response code="200">Availability retrieved successfully.</response>
        /// <response code="400">Validation failed.</response>
        /// <response code="404">Meeting room not found or is not active.</response>
        [HttpGet("{id}/availability")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseWrapper<List<TimeSlotResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseWrapper<List<TimeSlotResponse>>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAvailability([FromRoute] int id, [FromQuery] GetRoomAvailabilityRequest request, CancellationToken cancellationToken)
        {
            var response = await _meetingRoomService.GetAvailability(id, request, cancellationToken);

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Retrieves the occupied hours for a room on a given date, within a given time window.
        /// </summary>
        /// <remarks>
        /// date, fromTime, and toTime are local to timeZoneId. Only overlapping Confirmed bookings
        /// are returned - Pending, Rejected, and Cancelled bookings are never occupying.
        /// </remarks>
        /// <response code="200">Occupied hours retrieved successfully.</response>
        /// <response code="400">Validation failed.</response>
        /// <response code="404">Meeting room not found or is not active.</response>
        [HttpGet("{id}/occupied-hours")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseWrapper<List<TimeSlotResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseWrapper<List<TimeSlotResponse>>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOccupiedHours([FromRoute] int id, [FromQuery] GetOccupiedHoursRequest request, CancellationToken cancellationToken)
        {
            var response = await _meetingRoomService.GetOccupiedHours(id, request, cancellationToken);

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Updates a meeting room. Any field may be omitted (or null) to leave it unchanged.
        /// </summary>
        /// <remarks>
        /// Also used to activate/deactivate a room via the isActive field. Administrator role required.
        /// </remarks>
        /// <response code="200">Meeting room updated successfully.</response>
        /// <response code="400">Validation failed, or the resulting opening time is not before the closing time.</response>
        /// <response code="404">Meeting room not found.</response>
        /// <response code="409">A meeting room with this name already exists at this location.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        [ValidateApiAntiforgeryToken]
        [ProducesResponseType(typeof(ResponseWrapper<MeetingRoomsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseWrapper<MeetingRoomsResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseWrapper<MeetingRoomsResponse>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateMeetingRoomRequest request, CancellationToken cancellationToken)
        {
            var response = await _meetingRoomService.Update(id, request, cancellationToken);

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Retrieves every meeting room, including deactivated ones.
        /// </summary>
        /// <remarks>
        /// Administrator role required. This is the unfiltered admin view - see
        /// GET /api/meetingroom for the active-only employee catalog.
        /// </remarks>
        /// <response code="200">Meeting rooms retrieved successfully.</response>
        [HttpGet("admin")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(ResponseWrapper<List<MeetingRoomsResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllForAdmin(CancellationToken cancellationToken)
        {
            var response = await _meetingRoomService.GetAllForAdmin(cancellationToken);

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Retrieves a single meeting room by id, including deactivated ones.
        /// </summary>
        /// <remarks>
        /// Administrator role required. This is the unfiltered admin view - see
        /// GET /api/meetingroom/{id} for the active-only employee lookup.
        /// </remarks>
        /// <response code="200">Meeting room retrieved successfully.</response>
        /// <response code="404">Meeting room not found.</response>
        [HttpGet("admin/{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(ResponseWrapper<MeetingRoomsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<MeetingRoomsResponse>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdForAdmin([FromRoute] int id, CancellationToken cancellationToken)
        {
            var response = await _meetingRoomService.GetByIdForAdmin(id, cancellationToken);

            return StatusCode(response.StatusCode, response);
        }
    }
}
