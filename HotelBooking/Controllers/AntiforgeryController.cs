using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AntiforgeryController(IAntiforgery antiforgery) : ControllerBase
    {
        /// <summary>
        /// Issues an antiforgery token pair - the request token is returned in the body,
        /// the matching cookie is set on the response.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/antiforgery/token
        ///
        /// Send the returned token back on state-changing requests via the X-CSRF-TOKEN header.
        /// </remarks>
        /// <response code="200">Token issued.</response>
        [HttpGet("token")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetToken()
        {
            var tokens = antiforgery.GetAndStoreTokens(HttpContext);
            return Ok(new { token = tokens.RequestToken });
        }
    }
}
