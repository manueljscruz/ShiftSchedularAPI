using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    /// <summary>
    /// Repository implementation for HolidayCatalogLocalization entity operations.
    /// Handles database operations for holiday catalog localized names and descriptions.
    /// </summary>
    public class HolidayCatalogLocalizationRepository : GenericRepository<HolidayCatalogLocalization>, IHolidayCatalogLocalizationRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<HolidayCatalogLocalization> _dbSet;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationRepository _localizationRepository;

        /// <summary>
        /// Initializes a new instance of the HolidayCatalogLocalizationRepository.
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="unitOfWork">The unit of work for transaction management</param>
        public HolidayCatalogLocalizationRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _localizationRepository = _unitOfWork.LocalizationRepository;
            _dbSet = _context.Set<HolidayCatalogLocalization>();
        }

        /// <summary>
        /// Retrieves all holiday catalog entries with their localized values for the specified language.
        /// Includes related HolidayCatalog with HolidayType and HolidayBehaviour navigation properties.
        /// </summary>
        /// <param name="languageCode">The language code (e.g., "en", "pt", "es")</param>
        /// <returns>A collection of HolidayCatalogLocalization entities for the specified language</returns>
        public async Task<IEnumerable<HolidayCatalogLocalization>> GetHolidayCatalogsByLocalization(string languageCode)
        {
            // Return empty if no language code provided
            if (string.IsNullOrWhiteSpace(languageCode))
                return Enumerable.Empty<HolidayCatalogLocalization>();

            // Get the localization record for the specified language
            Localization localization = await _localizationRepository.GetLocalizationByLanguageCode(languageCode);

            if (localization != null)
            {
                return await _dbSet
                    .Include(hcl => hcl.HolidayCatalog)
                        .ThenInclude(hc => hc.HolidayType)
                    .Include(hcl => hcl.HolidayCatalog)
                        .ThenInclude(hc => hc.HolidayBehaviour)
                    .Where(hcl => hcl.LocalizationId.Equals(localization.LocalizationId))
                    .ToListAsync();
            }

            return Enumerable.Empty<HolidayCatalogLocalization>();
        }
    }
}
