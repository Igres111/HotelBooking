using DotNetEnv;
using HotelBooking.DI;
using HotelBooking.Middleware;
using Serilog;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day));

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

builder.Services.AddDatabase(connectionString);
builder.Services.AddApiCore();
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();
builder.Services.AddValidators();
builder.Services.AddAppAuthentication();
builder.Services.AddAppAntiforgery();
builder.Services.AddSwaggerDocumentation();

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
