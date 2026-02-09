using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    /// <summary>
    /// Repository interface for HolidayBehaviourLocalization entity operations.
    /// Provides methods to retrieve holiday behaviours with their localized display values.
    /// </summary>
    public interface IHolidayBehaviourLocalizationRepository : IGenericRepository<HolidayBehaviourLocalization>
    {
        /// <summary>
        /// Retrieves all holiday behaviours with their localized display values for the specified language.
        /// </summary>
        /// <param name="languageCode">The language code (e.g., "en", "pt", "es")</param>
        /// <returns>A collection of HolidayBehaviourLocalization entities for the specified language</returns>
        Task<IEnumerable<HolidayBehaviourLocalization>> GetHolidayBehavioursByLocalization(string languageCode);
    }
}
