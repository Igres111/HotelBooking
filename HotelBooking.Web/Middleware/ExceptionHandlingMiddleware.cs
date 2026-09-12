using System.Net;
using System.Text.Json;
using HotelBooking.Web.Models.Responses;

namespace HotelBooking.Web.Middleware
{
    public class ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                logger.LogInformation(
                    "Request {Method} {Path} was cancelled by the client.",
                    context.Request.Method,
                    context.Request.Path);
            }
            catch (Exception exception)
            {
                if (context.Response.HasStarted)
                {
                    logger.LogError(
                        exception,
                        "Unhandled exception occurred while processing {Method} {Path}",
                        context.Request.Method,
                        context.Request.Path);

                    throw;
                }

                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = GetStatusCode(exception);
            var message = GetMessage(exception);

            if (statusCode == HttpStatusCode.InternalServerError)
            {
                logger.LogError(
                    exception,
                    "Unhandled exception occurred while processing {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);
            }
            else
            {
                logger.LogWarning(
                    exception,
                    "Request {Method} {Path} failed with {StatusCode}",
                    context.Request.Method,
                    context.Request.Path,
                    (int)statusCode);
            }

            context.Response.Clear();
            context.Response.StatusCode = (int)statusCode;

            if (PrefersJson(context))
            {
                context.Response.ContentType = "application/json; charset=utf-8";

                var response = new ApiErrorResponse(
                    (int)statusCode,
                    message,
                    environment.IsDevelopment() ? exception.ToString() : "");

                var json = JsonSerializer.Serialize(
                    response,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                await context.Response.WriteAsync(json);

                return;
            }

            context.Response.Redirect($"/Home/Error?statusCode={(int)statusCode}");
        }

        private static bool PrefersJson(HttpContext context) =>
            context.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
            context.Request.Headers.Accept.Any(value => value is not null && value.Contains("application/json", StringComparison.OrdinalIgnoreCase));

        private static HttpStatusCode GetStatusCode(Exception exception) => exception switch
        {
            HttpRequestException => HttpStatusCode.ServiceUnavailable,
            TaskCanceledException => HttpStatusCode.GatewayTimeout,
            JsonException => HttpStatusCode.BadGateway,
            _ => HttpStatusCode.InternalServerError
        };

        private static string GetMessage(Exception exception) => exception switch
        {
            HttpRequestException => "The booking service is currently unavailable. Please try again later.",
            TaskCanceledException => "The request to the booking service timed out. Please try again.",
            JsonException => "Received an invalid response from the booking service.",
            _ => "An unexpected error occurred."
        };
    }
}
