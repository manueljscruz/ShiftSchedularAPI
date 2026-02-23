using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;

namespace ShiftSchedularDAL.IRepositories
{
    /// <summary>
    /// Repository interface for EntityHoliday entity operations.
    /// Provides methods to manage entity-specific holidays including both catalog-subscribed and custom holidays.
    /// </summary>
    public interface IEntityHolidayRepository : IGenericRepository<EntityHoliday>
    {
        /// <summary>
        /// Retrieves all holidays associated with a specific entity.
        /// </summary>
        /// <param name="entityId">The unique identifier of the entity</param>
        /// <returns>A collection of EntityHoliday records for the specified entity</returns>
        Task<IEnumerable<EntityHoliday>> GetEntityHolidays(Guid entityId, string languageCode);

        /// <summary>
        /// Retrieves a specific entity holiday by its unique identifier.
        /// Includes related HolidayCatalog and HolidayBehaviour navigation properties.
        /// </summary>
        /// <param name="entityHolidayId">The unique identifier of the entity holiday</param>
        /// <returns>The EntityHoliday record if found, otherwise null</returns>
        Task<EntityHoliday> GetEntityHolidayById(Guid entityHolidayId, string languageCode);

        /// <summary>
        /// Retrieves a paginated list of holidays for a specific entity.
        /// </summary>
        /// <param name="entityId">The unique identifier of the entity</param>
        /// <param name="pageNumber">The page number (1-based)</param>
        /// <param name="pageSize">The number of records per page</param>
        /// <returns>A paginated list of EntityHoliday records</returns>
        Task<PagedList<EntityHoliday>> GetEntityHolidaysPaginated(Guid entityId, int pageNumber, int pageSize, string languageCode);
    }
}
