using Microsoft.Data.SqlClient;
using Serilog;
using System.Net;
using System.Text.Json;

namespace ShiftSchedularAPI.Middleware
{
    /// <summary>
    /// Global exception handling middleware that catches unhandled exceptions,
    /// logs them with Serilog, and returns consistent JSON error responses.
    /// </summary>
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, IHostEnvironment environment)
        {
            _next = next;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var correlationId = Guid.NewGuid().ToString();

            // Log the exception with full details
            Log.Error(exception,
                "Unhandled exception occurred. CorrelationId: {CorrelationId}, Method: {Method}, Path: {Path}, QueryString: {QueryString}",
                correlationId,
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString);

            // Determine status code and message based on exception type
            var (statusCode, message) = exception switch
            {
                SqlException sqlEx => (
                    HttpStatusCode.InternalServerError,
                    "A database error occurred. Please try again later."),

                UnauthorizedAccessException => (
                    HttpStatusCode.Unauthorized,
                    "You are not authorized to perform this action."),

                ArgumentNullException or ArgumentException => (
                    HttpStatusCode.BadRequest,
                    "Invalid request parameters."),

                KeyNotFoundException => (
                    HttpStatusCode.NotFound,
                    "The requested resource was not found."),

                InvalidOperationException => (
                    HttpStatusCode.BadRequest,
                    "The operation could not be completed."),

                TimeoutException => (
                    HttpStatusCode.RequestTimeout,
                    "The request timed out. Please try again."),

                _ => (
                    HttpStatusCode.InternalServerError,
                    "An unexpected error occurred. Please try again later.")
            };

            // Build response object
            var response = new
            {
                success = false,
                message = message,
                correlationId = correlationId,
                // Include detailed error info only in non-production environments
                detail = !_environment.IsProduction() ? new
                {
                    exceptionType = exception.GetType().Name,
                    exceptionMessage = exception.Message,
                    stackTrace = exception.StackTrace,
                    innerException = exception.InnerException?.Message
                } : null
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
        }
    }

    /// <summary>
    /// Extension method to register the middleware
    /// </summary>
    public static class GlobalExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}
