namespace ShiftSchedularAPI.Middleware
{
    /// <summary>
    /// Middleware that extracts the language code from the Accept-Language header
    /// and stores it in HttpContext.Items for use throughout the request pipeline.
    /// </summary>
    public class LanguageMiddleware
    {
        private readonly RequestDelegate _next;

        public LanguageMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var acceptLanguageHeader = context.Request.Headers["Accept-Language"].FirstOrDefault();

            // Parse the Accept-Language header
            // Examples: "en", "en-US", "pt-BR,pt;q=0.9,en;q=0.8"
            var languageCode = ParseLanguageCode(acceptLanguageHeader);

            context.Items["LanguageCode"] = languageCode;
            await _next(context);
        }

        /// <summary>
        /// Parses the Accept-Language header and extracts the primary language code.
        /// </summary>
        /// <param name="acceptLanguageHeader">The Accept-Language header value</param>
        /// <returns>The two-letter language code (e.g., "en", "pt", "es")</returns>
        private static string ParseLanguageCode(string? acceptLanguageHeader)
        {
            if (string.IsNullOrWhiteSpace(acceptLanguageHeader))
            {
                return "en";
            }

            // Take the first language preference (before any comma)
            var primaryLanguage = acceptLanguageHeader.Split(',').FirstOrDefault()?.Trim();

            if (string.IsNullOrWhiteSpace(primaryLanguage))
            {
                return "en";
            }

            // Remove quality factor if present (e.g., "en-US;q=0.9" -> "en-US")
            var languageWithoutQuality = primaryLanguage.Split(';').FirstOrDefault()?.Trim();

            if (string.IsNullOrWhiteSpace(languageWithoutQuality))
            {
                return "en";
            }

            // Extract the primary language code (e.g., "en-US" -> "en", "pt-BR" -> "pt")
            var languageCode = languageWithoutQuality.Split('-').FirstOrDefault()?.Trim().ToLowerInvariant();

            return string.IsNullOrWhiteSpace(languageCode) ? "en" : languageCode;
        }
    }

    /// <summary>
    /// Extension method to register the LanguageMiddleware in the application pipeline.
    /// </summary>
    public static class LanguageMiddlewareExtensions
    {
        public static IApplicationBuilder UseLanguageMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LanguageMiddleware>();
        }
    }
}
