namespace HotelBooking.DI
{
    public static class ApiExtensions
    {
        public static IServiceCollection AddApiCore(this IServiceCollection services)
        {
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            return services;
        }
    }
}
