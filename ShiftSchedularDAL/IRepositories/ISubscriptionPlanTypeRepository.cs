using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    /// <summary>
    /// Repository interface for SubscriptionPlanType entity operations.
    /// </summary>
    public interface ISubscriptionPlanTypeRepository : IGenericRepository<SubscriptionPlanType>
    {
        /// <summary>
        /// Retrieves all subscription plan types with their localizations eagerly loaded.
        /// </summary>
        Task<IEnumerable<SubscriptionPlanType>> GetAllWithLocalizations();

        /// <summary>
        /// Retrieves a subscription plan type by id with its localizations eagerly loaded.
        /// </summary>
        Task<SubscriptionPlanType> GetByIdWithLocalizations(int id);
    }
}
