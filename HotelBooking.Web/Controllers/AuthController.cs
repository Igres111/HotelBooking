using System.Security.Claims;
using HotelBooking.Web.Handlers;
using HotelBooking.Web.Models.Enums;
using HotelBooking.Web.Models.ViewModels;
using HotelBooking.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpViewModel model, CancellationToken cancellationToken)
        {
            var validationResult = await _authService.SignUp(model, cancellationToken);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return View(model);
            }

            TempData["SuccessMessage"] = "Account created successfully. You can now log in.";
            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return DefaultLandingPage(User.IsInRole(nameof(UserRole.Administrator)));
            }

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
        {
            var result = await _authService.Login(model, cancellationToken);

            if (!result.ValidationResult.IsValid)
            {
                foreach (var error in result.ValidationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return View(model);
            }

            var user = result.User!;
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role.ToString())
            };

            if (!string.IsNullOrEmpty(result.ApiAuthCookie))
            {
                claims.Add(new Claim(ApiAuthCookieHandler.ClaimType, result.ApiAuthCookie));
            }

            if (!string.IsNullOrEmpty(result.ApiAntiforgeryCookie))
            {
                claims.Add(new Claim(ApiAuthCookieHandler.AntiforgeryCookieClaimType, result.ApiAntiforgeryCookie));
            }

            if (!string.IsNullOrEmpty(result.ApiAntiforgeryToken))
            {
                claims.Add(new Claim(ApiAuthCookieHandler.AntiforgeryTokenClaimType, result.ApiAntiforgeryToken));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            TempData["SuccessMessage"] = "Logged in successfully.";

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return DefaultLandingPage(user.Role == UserRole.Administrator);
        }

        private IActionResult DefaultLandingPage(bool isAdmin)
        {
            return isAdmin ? RedirectToAction("Dashboard", "Admin") : RedirectToAction("Index", "Booking");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            await _authService.Logout(cancellationToken);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["SuccessMessage"] = "Logged out successfully.";
            return RedirectToAction("Login", "Auth");
        }
    }
}
