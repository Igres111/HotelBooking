using FluentValidation;
using HotelBooking.Models.Requests;
using HotelBooking.Validators;

namespace HotelBooking.DI
{
    public static class ValidationExtensions
    {
        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<RegisterUserRequest>, RegisterUserRequestValidator>();
            services.AddScoped<IValidator<LoginUserRequest>, LoginUserRequestValidator>();
            services.AddScoped<IValidator<CreateMeetingRoomRequest>, CreateMeetingRoomRequestValidator>();
            services.AddScoped<IValidator<UpdateMeetingRoomRequest>, UpdateMeetingRoomRequestValidator>();
            services.AddScoped<IValidator<CreateBookingRequest>, CreateBookingRequestValidator>();
            services.AddScoped<IValidator<CreateRecurringBookingRequest>, CreateRecurringBookingRequestValidator>();
            services.AddScoped<IValidator<GetRoomAvailabilityRequest>, GetRoomAvailabilityRequestValidator>();
            services.AddScoped<IValidator<GetOccupiedHoursRequest>, GetOccupiedHoursRequestValidator>();
            services.AddScoped<IValidator<GetDashboardRequest>, GetDashboardRequestValidator>();

            return services;
        }
    }
}
