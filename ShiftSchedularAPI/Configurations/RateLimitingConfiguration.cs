using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using ShiftSchedularEntity.Models;
using System.Threading.RateLimiting;

namespace ShiftSchedularAPI.Configurations
{
    public static class RateLimitingConfiguration
    {
        public static IServiceCollection AddCustomRateLimiting(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<RateLimitSettings>(
                configuration.GetSection("RateLimitSettings"));

            services.AddRateLimiter(options =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var settings = serviceProvider.GetRequiredService<IOptions<RateLimitSettings>>().Value;

                // 1. AUTHENTICATION ENDPOINTS - Very strict
                // 5 attempts in 1 minute
                options.AddPolicy("auth", context =>
                {
                    var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: ipAddress,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromMinutes(1),
                            PermitLimit = 5,
                            QueueLimit = 0
                        });
                });

                // 2. Forgot Password - Strict to prevent abuse
                // 3 attempts in 15 minutes per IP address
                options.AddPolicy("forgot-password", context =>
                {
                    var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: ipAddress,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromMinutes(settings.ForgotPassword.WindowMinutes),
                            PermitLimit = settings.ForgotPassword.PermitLimit,
                            QueueLimit = 0
                        });
                });

                // 3. Refresh Token - Moderate (30 per hour)
                options.AddPolicy("refresh", context =>
                {
                    // Rate limit by user ID from token or IP as fallback
                    var userId = context.User.Identity?.Name
                        ?? context.Connection.RemoteIpAddress?.ToString()
                        ?? "unknown";

                    return RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: userId,
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromHours(1),
                            PermitLimit = 30,
                            SegmentsPerWindow = 6,
                            QueueLimit = 0
                        });
                });

                // 3. Logout - Lenient (10 per hour)
                options.AddPolicy("logout", context =>
                {
                    var userId = context.User.Identity?.Name
                        ?? context.Connection.RemoteIpAddress?.ToString()
                        ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: userId,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromHours(1),
                            PermitLimit = 10,
                            QueueLimit = 0
                        });
                });

                // 4. GENERAL API - Default for most endpoints
                options.AddPolicy("general", context =>
                {
                    var userId = context.User.Identity?.IsAuthenticated == true
                        ? context.User.Identity.Name ?? "anonymous"
                        : context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

                    var limit = context.User.Identity?.IsAuthenticated == true ? 100 : 20;

                    return RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: userId,
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromMinutes(1),
                            PermitLimit = limit,
                            SegmentsPerWindow = 6,
                            QueueLimit = 0
                        });
                });

                // 5. READ-ONLY/PUBLIC - More relaxed
                options.AddPolicy("public", context =>
                {
                    var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: ipAddress,
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromMinutes(1),
                            PermitLimit = 200,
                            SegmentsPerWindow = 6,
                            QueueLimit = 0
                        });
                });

                // 6. Write operations (mutations) - Moderate (30/min)
                options.AddPolicy("write", context =>
                {
                    var userId = context.User.Identity?.Name ?? "anonymous";
                    return RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: userId,
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromMinutes(1),
                            PermitLimit = 30, // 30 template selections per minute
                            SegmentsPerWindow = 6,
                            QueueLimit = 0
                        });
                });

                // 7. RESOURCE-INTENSIVE - Custom strict limits
                options.AddPolicy("heavy", context =>
                {
                    var userId = context.User.Identity?.Name ?? "anonymous";
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: userId,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromMinutes(1),
                            PermitLimit = 10,
                            QueueLimit = 0
                        });
                });

                ConfigureRejectionHandler(options);
            });


            return services;
        }

        private static void ConfigureRejectionHandler(RateLimiterOptions options)
        {
            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds).ToString();
                }

                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    error = "Too many requests. Please try again later.",
                    retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retry)
                        ? (int)retry.TotalSeconds
                        : (int?)null
                }, cancellationToken: token);
            };
        }
    }
}
