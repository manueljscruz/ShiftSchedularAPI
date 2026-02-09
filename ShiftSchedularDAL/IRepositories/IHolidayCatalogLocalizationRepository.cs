using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    /// <summary>
    /// Repository interface for HolidayCatalogLocalization entity operations.
    /// Provides methods to retrieve holiday catalog entries with their localized names and descriptions.
    /// </summary>
    public interface IHolidayCatalogLocalizationRepository : IGenericRepository<HolidayCatalogLocalization>
    {
        /// <summary>
        /// Retrieves all holiday catalog entries with their localized values for the specified language.
        /// </summary>
        /// <param name="languageCode">The language code (e.g., "en", "pt", "es")</param>
        /// <returns>A collection of HolidayCatalogLocalization entities for the specified language</returns>
        Task<IEnumerable<HolidayCatalogLocalization>> GetHolidayCatalogsByLocalization(string languageCode);
    }
}
