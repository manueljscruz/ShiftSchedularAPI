using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    /// <summary>
    /// Repository interface for SubscriptionDurationType entity operations.
    /// </summary>
    public interface ISubscriptionDurationTypeRepository : IGenericRepository<SubscriptionDurationType>
    {
        /// <summary>
        /// Retrieves all subscription duration types with their localizations eagerly loaded.
        /// </summary>
        Task<IEnumerable<SubscriptionDurationType>> GetAllWithLocalizations();

        /// <summary>
        /// Retrieves a subscription duration type by id with its localizations eagerly loaded.
        /// </summary>
        Task<SubscriptionDurationType> GetByIdWithLocalizations(int id);
    }
}
