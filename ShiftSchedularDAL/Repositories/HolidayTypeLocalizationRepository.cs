using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    /// <summary>
    /// Repository implementation for HolidayTypeLocalization entity operations.
    /// Handles database operations for holiday type localized display values.
    /// </summary>
    public class HolidayTypeLocalizationRepository : GenericRepository<HolidayTypeLocalization>, IHolidayTypeLocalizationRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<HolidayTypeLocalization> _dbSet;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationRepository _localizationRepository;

        /// <summary>
        /// Initializes a new instance of the HolidayTypeLocalizationRepository.
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="unitOfWork">The unit of work for transaction management</param>
        public HolidayTypeLocalizationRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _localizationRepository = _unitOfWork.LocalizationRepository;
            _dbSet = _context.Set<HolidayTypeLocalization>();
        }

        /// <summary>
        /// Retrieves all holiday types with their localized display values for the specified language.
        /// </summary>
        /// <param name="languageCode">The language code (e.g., "en", "pt", "es")</param>
        /// <returns>A collection of HolidayTypeLocalization entities for the specified language</returns>
        public async Task<IEnumerable<HolidayTypeLocalization>> GetHolidayTypesByLocalization(string languageCode)
        {
            // Return empty if no language code provided
            if (string.IsNullOrWhiteSpace(languageCode))
                return Enumerable.Empty<HolidayTypeLocalization>();

            // Get the localization record for the specified language
            Localization localization = await _localizationRepository.GetLocalizationByLanguageCode(languageCode);

            if (localization != null)
                return await _dbSet.Where(htl => htl.LocalizationId.Equals(localization.LocalizationId)).ToListAsync();

            return Enumerable.Empty<HolidayTypeLocalization>();
        }
    }
}
