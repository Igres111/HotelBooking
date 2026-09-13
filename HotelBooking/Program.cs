using DotNetEnv;
using FluentValidation;
using HotelBooking.Data;
using HotelBooking.Middleware;
using HotelBooking.Models.Requests;
using HotelBooking.Repositories;
using HotelBooking.Repositories.BaseRepository;
using HotelBooking.Repositories.Interfaces;
using HotelBooking.Services;
using HotelBooking.Services.Interfaces;
using HotelBooking.Validators;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Serilog;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day));

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMeetingRoomRepository, MeetingRoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IIdempotencyRecordRepository, IdempotencyRecordRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMeetingRoomService, MeetingRoomService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddSingleton<ITimeZoneConverter, TimeZoneConverter>();
builder.Services.AddScoped<IValidator<RegisterUserRequest>, RegisterUserRequestValidator>();
builder.Services.AddScoped<IValidator<LoginUserRequest>, LoginUserRequestValidator>();
builder.Services.AddScoped<IValidator<CreateMeetingRoomRequest>, CreateMeetingRoomRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateMeetingRoomRequest>, UpdateMeetingRoomRequestValidator>();
builder.Services.AddScoped<IValidator<CreateBookingRequest>, CreateBookingRequestValidator>();
builder.Services.AddScoped<IValidator<CreateRecurringBookingRequest>, CreateRecurringBookingRequestValidator>();
builder.Services.AddScoped<IValidator<GetRoomAvailabilityRequest>, GetRoomAvailabilityRequestValidator>();
builder.Services.AddScoped<IValidator<GetOccupiedHoursRequest>, GetOccupiedHoursRequestValidator>();
builder.Services.AddScoped<IValidator<GetDashboardRequest>, GetDashboardRequestValidator>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "HotelBooking.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        return Task.CompletedTask;
    });

    await next();
});

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

Log.CloseAndFlush();
