using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;

namespace ShiftSchedularDAL.Repositories
{
    /// <summary>
    /// Repository implementation for EntityHoliday entity operations.
    /// Handles database operations for entity-specific holidays including both catalog-subscribed and custom holidays.
    /// </summary>
    public class EntityHolidayRepository : GenericRepository<EntityHoliday>, IEntityHolidayRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<EntityHoliday> _dbSet;
        private readonly IUnitOfWork _unitOfWork;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the EntityHolidayRepository.
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="unitOfWork">The unit of work for transaction management</param>
        public EntityHolidayRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _dbSet = _context.Set<EntityHoliday>();
        }

        #endregion

        #region Get Entity Holidays

        /// <summary>
        /// Retrieves all holidays associated with a specific entity.
        /// Includes HolidayCatalog and HolidayBehaviour navigation properties.
        /// </summary>
        /// <param name="entityId">The unique identifier of the entity</param>
        /// <returns>A collection of EntityHoliday records for the specified entity</returns>
        public async Task<IEnumerable<EntityHoliday>> GetEntityHolidays(Guid entityId, string languageCode, bool includeInactive = false, DateTime? startDateSearch = null, DateTime? endDateSearch = null)
        {
            if (entityId == Guid.Empty)
                return Enumerable.Empty<EntityHoliday>();

            Localization localization = await _unitOfWork.LocalizationRepository.GetLocalizationByLanguageCode(languageCode);

            var results = await _dbSet
                .Include(eh => eh.HolidayCatalog)
                    .ThenInclude(hc => hc.HolidayCatalogLocalizations.Where(i => i.LocalizationId.Equals(localization.LocalizationId)))
                 .Include(eh => eh.HolidayCatalog)
                    .ThenInclude(hc => hc.HolidayType)
                        .ThenInclude(ht => ht.HolidayTypeLocalizations.Where(i => i.LocalizationId.Equals(localization.LocalizationId)))
                .Include(eh => eh.HolidayCatalog)
                    .ThenInclude(hc => hc.HolidayBehaviour)
                        .ThenInclude(hb => hb.HolidayBehaviourLocalizations.Where(i => i.LocalizationId.Equals(localization.LocalizationId)))
                .Include(eh => eh.HolidayBehaviour)
                    .ThenInclude(hb => hb.HolidayBehaviourLocalizations.Where(i => i.LocalizationId.Equals(localization.LocalizationId)))
                .Where(eh => eh.EntityId.Equals(entityId) && (includeInactive || eh.IsActive)).ToListAsync();

            if(startDateSearch != null && endDateSearch != null)
            {
                int startYear = startDateSearch.Value.Year;
                int endYear = endDateSearch.Value.Year;

                return results.Where(i =>
                    Enumerable.Range(startYear, endYear - startYear + 1)
                        .Any(year =>
                        {
                            DateTime holidayDate = GetHolidayDate(i, year);
                            return holidayDate >= startDateSearch.Value && holidayDate <= endDateSearch.Value;
                        })
                );
            }

            return results;
        }

        #endregion

        #region Aux : Get Holiday Date

        /// <summary>
        /// Used to facilitate the filtering of entity holidays by dates
        /// Checks if its a custom holiday vs an existing holiday from the catalog
        /// </summary>
        /// <param name="h"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private DateTime GetHolidayDate(EntityHoliday h, int year) =>
            h.HolidayCatalog != null
                ? new DateTime(year, h.HolidayCatalog.RecurrenceMonth, h.HolidayCatalog.RecurrenceDay)
                : new DateTime(year, h.CustomMonth, h.CustomDay);

        #endregion

        #region Get By Entity Id

        /// <inheritdoc/>
        public async Task<IEnumerable<EntityHoliday>> GetByEntityId(Guid entityId)
        {
            if (entityId == Guid.Empty)
                return Enumerable.Empty<EntityHoliday>();

            return await _dbSet
                .Where(eh => eh.EntityId.Equals(entityId))
                .ToListAsync();
        }

        #endregion

        #region Get Entity Holiday By Id

        /// <summary>
        /// Retrieves a specific entity holiday by its unique identifier.
        /// Includes related HolidayCatalog and HolidayBehaviour navigation properties.
        /// </summary>
        /// <param name="entityHolidayId">The unique identifier of the entity holiday</param>
        /// <returns>The EntityHoliday record if found, otherwise null</returns>
        public async Task<EntityHoliday> GetEntityHolidayById(Guid entityHolidayId, string languageCode)
        {
            if (entityHolidayId == Guid.Empty)
                return null;

            Localization localization = await _unitOfWork.LocalizationRepository.GetLocalizationByLanguageCode(languageCode);

            return await _dbSet
                .Include(eh => eh.HolidayCatalog)
                    .ThenInclude(hc => hc.HolidayCatalogLocalizations.Where(i => i.LocalizationId.Equals(localization.LocalizationId)))
                 .Include(eh => eh.HolidayCatalog)
                    .ThenInclude(hc => hc.HolidayType)
                        .ThenInclude(ht => ht.HolidayTypeLocalizations.Where(i => i.LocalizationId.Equals(localization.LocalizationId)))
                .Include(eh => eh.HolidayCatalog)
                    .ThenInclude(hc => hc.HolidayBehaviour)
                        .ThenInclude(hb => hb.HolidayBehaviourLocalizations.Where(i => i.LocalizationId.Equals(localization.LocalizationId)))
                .Include(eh => eh.HolidayBehaviour)
                    .ThenInclude(hb => hb.HolidayBehaviourLocalizations.Where(i => i.LocalizationId.Equals(localization.LocalizationId)))
                .FirstOrDefaultAsync(eh => eh.EntityHolidayId.Equals(entityHolidayId));
        }

        #endregion

        #region Get Entity Holidays Paginated

        /// <summary>
        /// Retrieves a paginated list of holidays for a specific entity.
        /// Includes HolidayCatalog and HolidayBehaviour navigation properties.
        /// </summary>
        /// <param name="entityId">The unique identifier of the entity</param>
        /// <param name="pageNumber">The page number (1-based)</param>
        /// <param name="pageSize">The number of records per page</param>
        /// <returns>A paginated list of EntityHoliday records</returns>
        public async Task<PagedList<EntityHoliday>> GetEntityHolidaysPaginated(Guid entityId, int pageNumber, int pageSize, string languageCode, bool includeInactive)
        {
            if (entityId == Guid.Empty)
                return PagedList<EntityHoliday>.CreateEmpty();

            Localization localization = await _unitOfWork.LocalizationRepository.GetLocalizationByLanguageCode(languageCode);

            // Build the base query
            var baseQuery = _dbSet
                .Include(eh => eh.HolidayCatalog)
                    .ThenInclude(hc => hc.HolidayCatalogLocalizations.Where(i => i.LocalizationId.Equals(localization.LocalizationId)))
                 .Include(eh => eh.HolidayCatalog)
                    .ThenInclude(hc => hc.HolidayType)
                        .ThenInclude(ht => ht.HolidayTypeLocalizations.Where(i => i.LocalizationId.Equals(localization.LocalizationId)))
                .Include(eh => eh.HolidayCatalog)
                    .ThenInclude(hc => hc.HolidayBehaviour)
                        .ThenInclude(hb => hb.HolidayBehaviourLocalizations.Where(i => i.LocalizationId.Equals(localization.LocalizationId)))
                .Include(eh => eh.HolidayBehaviour)
                    .ThenInclude(hb => hb.HolidayBehaviourLocalizations.Where(i => i.LocalizationId.Equals(localization.LocalizationId)))
                .Where(eh => eh.EntityId.Equals(entityId) && (includeInactive || eh.IsActive))
                .OrderBy(eh => eh.CustomMonth)
                .ThenBy(eh => eh.CustomDay);

            // Get total count asynchronously
            int totalCount = await baseQuery.CountAsync();

            // Get paginated items asynchronously
            var pagedItems = await baseQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return PagedList<EntityHoliday>.Create(pagedItems.AsQueryable(), totalCount, pageNumber, pageSize);
        }

        #endregion
    }
}
