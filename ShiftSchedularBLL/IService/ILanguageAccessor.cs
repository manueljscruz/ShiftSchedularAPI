namespace ShiftSchedularBLL.IService
{
    /// <summary>
    /// Provides access to the current request's language code.
    /// The language code is extracted from the Accept-Language header by the LanguageMiddleware.
    /// </summary>
    public interface ILanguageAccessor
    {
        /// <summary>
        /// Gets the language code for the current request.
        /// </summary>
        /// <returns>The two-letter language code (e.g., "en", "pt", "es"). Defaults to "en" if not available.</returns>
        string GetLanguageCode();
    }
}
