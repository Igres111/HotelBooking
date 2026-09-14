using Microsoft.AspNetCore.Authentication.Cookies;

namespace HotelBooking.DI
{
    public static class SecurityExtensions
    {
        public static IServiceCollection AddAppAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "HotelBooking.Auth";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                    options.SlidingExpiration = true;
                });

            return services;
        }

        public static IServiceCollection AddAppAntiforgery(this IServiceCollection services)
        {
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
                options.Cookie.Name = "HotelBooking.Antiforgery";
            });

            return services;
        }
    }
}
