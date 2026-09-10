using System.Net;
using System.Security.Authentication;
using System.Text.Json;
using FluentValidation;
using HotelBooking.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Middleware
{
    public class ConflictException(string message) : Exception(message);

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

            var response = new ApiErrorResponse(
                (int)statusCode,
                GetMessage(exception),
                environment.IsDevelopment() ? exception.ToString() : "");

            context.Response.Clear();
            context.Response.StatusCode = response.StatusCode;
            context.Response.ContentType = "application/json; charset=utf-8";

            var json = JsonSerializer.Serialize(
                response,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            await context.Response.WriteAsync(json);
        }

        private static HttpStatusCode GetStatusCode(Exception exception) => exception switch
        {
            ValidationException => HttpStatusCode.BadRequest,
            ArgumentException => HttpStatusCode.BadRequest,
            KeyNotFoundException => HttpStatusCode.NotFound,
            AuthenticationException => HttpStatusCode.Unauthorized,
            UnauthorizedAccessException => HttpStatusCode.Forbidden,
            ConflictException => HttpStatusCode.Conflict,
            DbUpdateConcurrencyException => HttpStatusCode.Conflict,
            _ => HttpStatusCode.InternalServerError
        };

        private static string GetMessage(Exception exception) => exception switch
        {
            ValidationException validationException =>
                string.Join("; ", validationException.Errors.Select(error => error.ErrorMessage)),

            ArgumentException => exception.Message,

            KeyNotFoundException => exception.Message,

            AuthenticationException => exception.Message,

            UnauthorizedAccessException => "You do not have permission to perform this operation.",

            ConflictException => exception.Message,

            DbUpdateConcurrencyException => "The record was modified by another operation.",

            DbUpdateException => "A database error occurred.",

            _ => "An unexpected error occurred."
        };
    }
}