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

        /// <summary>
        /// Retrieves all holidays associated with a specific entity.
        /// Includes HolidayCatalog and HolidayBehaviour navigation properties.
        /// </summary>
        /// <param name="entityId">The unique identifier of the entity</param>
        /// <returns>A collection of EntityHoliday records for the specified entity</returns>
        public async Task<IEnumerable<EntityHoliday>> GetEntityHolidays(Guid entityId)
        {
            if (entityId == Guid.Empty)
                return Enumerable.Empty<EntityHoliday>();

            return await _dbSet
                .Include(eh => eh.HolidayCatalog)
                .Include(eh => eh.HolidayBehaviour)
                .Where(eh => eh.EntityId.Equals(entityId))
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific entity holiday by its unique identifier.
        /// Includes related HolidayCatalog and HolidayBehaviour navigation properties.
        /// </summary>
        /// <param name="entityHolidayId">The unique identifier of the entity holiday</param>
        /// <returns>The EntityHoliday record if found, otherwise null</returns>
        public async Task<EntityHoliday> GetEntityHolidayById(Guid entityHolidayId)
        {
            if (entityHolidayId == Guid.Empty)
                return null;

            return await _dbSet
                .Include(eh => eh.HolidayCatalog)
                .Include(eh => eh.HolidayBehaviour)
                .FirstOrDefaultAsync(eh => eh.EntityHolidayId.Equals(entityHolidayId));
        }

        /// <summary>
        /// Retrieves a paginated list of holidays for a specific entity.
        /// Includes HolidayCatalog and HolidayBehaviour navigation properties.
        /// </summary>
        /// <param name="entityId">The unique identifier of the entity</param>
        /// <param name="pageNumber">The page number (1-based)</param>
        /// <param name="pageSize">The number of records per page</param>
        /// <returns>A paginated list of EntityHoliday records</returns>
        public async Task<PagedList<EntityHoliday>> GetEntityHolidaysPaginated(Guid entityId, int pageNumber, int pageSize)
        {
            if (entityId == Guid.Empty)
                return PagedList<EntityHoliday>.CreateEmpty();

            // Build the base query
            var baseQuery = _dbSet
                .Include(eh => eh.HolidayCatalog)
                    //.ThenInclude(eh => eh.HolidayCatalogLocalizations)
                .Include(eh => eh.HolidayBehaviour)
                .Where(eh => eh.EntityId.Equals(entityId))
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
    }
}
