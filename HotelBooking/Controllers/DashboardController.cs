using HotelBooking.Models.Requests;
using HotelBooking.Models.Responses;
using HotelBooking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrator")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Retrieves booking statistics across every user.
        /// </summary>
        /// <remarks>
        /// Administrator role required.
        ///
        /// Sample request:
        ///
        ///     GET /api/dashboard?year=2026&amp;month=9
        ///
        /// year and month are both optional and filter by when the booking was made (CreatedAt), not
        /// when the meeting takes place. Passing month without year defaults to the current year.
        /// Passing year alone scopes to the whole year; passing neither returns all-time totals.
        ///
        /// TotalBookings sums every booking in scope that has not been soft-deleted, regardless of
        /// status or who created it. PendingBookings, ConfirmedBookings, and CancelledBookings each
        /// report the count for that specific status among the same set - Rejected bookings are
        /// included in TotalBookings but have no dedicated field.
        /// </remarks>
        /// <response code="200">Dashboard statistics retrieved successfully.</response>
        /// <response code="400">Month is outside the 1-12 range.</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseWrapper<DashboardResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetStatistics([FromQuery] GetDashboardRequest request, CancellationToken cancellationToken)
        {
            var response = await _dashboardService.GetStatistics(request, cancellationToken);

            return StatusCode(response.StatusCode, response);
        }
    }
}
