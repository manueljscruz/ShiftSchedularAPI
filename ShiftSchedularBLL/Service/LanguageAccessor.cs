using Microsoft.AspNetCore.Http;
using ShiftSchedularBLL.IService;

namespace ShiftSchedularBLL.Service
{
    /// <summary>
    /// Provides access to the current request's language code.
    /// The language code is extracted from the Accept-Language header by the LanguageMiddleware
    /// and stored in HttpContext.Items.
    /// </summary>
    public class LanguageAccessor : ILanguageAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LanguageAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets the language code for the current request.
        /// </summary>
        /// <returns>The two-letter language code (e.g., "en", "pt", "es"). Defaults to "en" if not available.</returns>
        public string GetLanguageCode()
        {
            var languageCode = _httpContextAccessor.HttpContext?.Items["LanguageCode"]?.ToString();
            return string.IsNullOrWhiteSpace(languageCode) ? "en" : languageCode;
        }
    }
}
