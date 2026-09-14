using HotelBooking.Data;
using HotelBooking.Repositories;
using HotelBooking.Repositories.BaseRepository;
using HotelBooking.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.DI
{
    public static class PersistenceExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, string? connectionString)
        {
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMeetingRoomRepository, MeetingRoomRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IIdempotencyRecordRepository, IdempotencyRecordRepository>();

            return services;
        }
    }
}
