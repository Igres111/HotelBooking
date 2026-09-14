using HotelBooking.Filters;
using HotelBooking.Models.Requests;
using HotelBooking.Models.Responses;
using HotelBooking.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelBooking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new employee account.
        /// </summary>
        /// <remarks>
        /// Always created with the Employee role - role cannot be set by the caller.
        /// </remarks>
        /// <response code="201">User registered successfully.</response>
        /// <response code="400">Validation failed.</response>
        /// <response code="409">A user with this email already exists.</response>
        [HttpPost("signup")]
        [AllowAnonymous]
        [ValidateApiAntiforgeryToken]
        [ProducesResponseType(typeof(ResponseWrapper<int>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseWrapper<int>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var response = await _authService.Register(request, cancellationToken);

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Authenticates a user and signs them in with a cookie-based session.
        /// </summary>
        /// <remarks>
        /// On success, an authentication cookie is issued.
        /// </remarks>
        /// <response code="200">Login successful.</response>
        /// <response code="401">Email or password is invalid.</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ValidateApiAntiforgeryToken]
        [ProducesResponseType(typeof(ResponseWrapper<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseWrapper<UserResponse>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
        {
            var response = await _authService.Login(request, cancellationToken);

            if (!response.IsSuccess)
            {
                return StatusCode(response.StatusCode, response);
            }

            var user = response.Data!;
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Signs the current user out and clears the authentication cookie.
        /// </summary>
        /// <response code="200">Logged out successfully.</response>
        [HttpPost("logout")]
        [Authorize]
        [ValidateApiAntiforgeryToken]
        [ProducesResponseType(typeof(ResponseWrapper), StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return StatusCode(StatusCodes.Status200OK, new ResponseWrapper(
                true,
                StatusCodes.Status200OK,
                "Logged out successfully."));
        }
    }
}
