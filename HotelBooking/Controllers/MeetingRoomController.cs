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
        /// Sample request:
        ///
        ///     POST /api/meetingroom
        ///     {
        ///         "name": "Conference Room A",
        ///         "description": "Large room with a projector",
        ///         "location": "Building 1, Floor 2",
        ///         "capacity": 10,
        ///         "openingTime": "08:00",
        ///         "closingTime": "18:00"
        ///     }
        ///
        /// Administrator role required.
        /// </remarks>
        /// <response code="201">Meeting room created successfully.</response>
        /// <response code="400">Validation failed.</response>
        /// <response code="409">A meeting room with this name already exists at this location.</response>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(ResponseWrapper<int>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseWrapper<int>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateMeetingRoomRequest request, CancellationToken cancellationToken)
        {
            var response = await _meetingRoomService.Create(request, cancellationToken);

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Retrieves every active meeting room.
        /// </summary>
        /// <remarks>
        /// Employee or Administrator role required. Deactivated rooms are excluded - use
        /// GET /api/meetingroom/admin for the unfiltered admin view.
        /// </remarks>
        /// <response code="200">Meeting rooms retrieved successfully.</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ResponseWrapper<List<MeetingRoomsResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllActive(CancellationToken cancellationToken)
        {
            var response = await _meetingRoomService.GetAllActive(cancellationToken);

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
        /// Updates a meeting room. Any field may be omitted (or null) to leave it unchanged.
        /// </summary>
        /// <remarks>
        /// Also used to activate/deactivate a room via the isActive field.
        ///
        /// Sample request:
        ///
        ///     PUT /api/meetingroom/1
        ///     {
        ///         "isActive": false
        ///     }
        ///
        /// Administrator role required.
        /// </remarks>
        /// <response code="200">Meeting room updated successfully.</response>
        /// <response code="400">Validation failed, or the resulting opening time is not before the closing time.</response>
        /// <response code="404">Meeting room not found.</response>
        /// <response code="409">A meeting room with this name already exists at this location.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
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
