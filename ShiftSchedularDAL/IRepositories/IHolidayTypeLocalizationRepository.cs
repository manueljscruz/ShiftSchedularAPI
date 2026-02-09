using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    /// <summary>
    /// Repository interface for HolidayTypeLocalization entity operations.
    /// Provides methods to retrieve holiday types with their localized display values.
    /// </summary>
    public interface IHolidayTypeLocalizationRepository : IGenericRepository<HolidayTypeLocalization>
    {
        /// <summary>
        /// Retrieves all holiday types with their localized display values for the specified language.
        /// </summary>
        /// <param name="languageCode">The language code (e.g., "en", "pt", "es")</param>
        /// <returns>A collection of HolidayTypeLocalization entities for the specified language</returns>
        Task<IEnumerable<HolidayTypeLocalization>> GetHolidayTypesByLocalization(string languageCode);
    }
}
