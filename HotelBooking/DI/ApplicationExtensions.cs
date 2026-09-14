using HotelBooking.Services;
using HotelBooking.Services.Interfaces;

namespace HotelBooking.DI
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IMeetingRoomService, MeetingRoomService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddSingleton<ITimeZoneConverter, TimeZoneConverter>();

            return services;
        }
    }
}
