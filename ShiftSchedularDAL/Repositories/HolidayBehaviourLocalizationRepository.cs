using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    /// <summary>
    /// Repository implementation for HolidayBehaviourLocalization entity operations.
    /// Handles database operations for holiday behaviour localized display values.
    /// </summary>
    public class HolidayBehaviourLocalizationRepository : GenericRepository<HolidayBehaviourLocalization>, IHolidayBehaviourLocalizationRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<HolidayBehaviourLocalization> _dbSet;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationRepository _localizationRepository;

        /// <summary>
        /// Initializes a new instance of the HolidayBehaviourLocalizationRepository.
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="unitOfWork">The unit of work for transaction management</param>
        public HolidayBehaviourLocalizationRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _localizationRepository = _unitOfWork.LocalizationRepository;
            _dbSet = _context.Set<HolidayBehaviourLocalization>();
        }

        /// <summary>
        /// Retrieves all holiday behaviours with their localized display values for the specified language.
        /// </summary>
        /// <param name="languageCode">The language code (e.g., "en", "pt", "es")</param>
        /// <returns>A collection of HolidayBehaviourLocalization entities for the specified language</returns>
        public async Task<IEnumerable<HolidayBehaviourLocalization>> GetHolidayBehavioursByLocalization(string languageCode)
        {
            // Return empty if no language code provided
            if (string.IsNullOrWhiteSpace(languageCode))
                return Enumerable.Empty<HolidayBehaviourLocalization>();

            // Get the localization record for the specified language
            Localization localization = await _localizationRepository.GetLocalizationByLanguageCode(languageCode);

            if (localization != null)
                return await _dbSet
                    .Include(hbl => hbl.HolidayBehaviour)
                    .Where(hbl => hbl.LocalizationId.Equals(localization.LocalizationId))
                    .ToListAsync();

            return Enumerable.Empty<HolidayBehaviourLocalization>();
        }
    }
}
