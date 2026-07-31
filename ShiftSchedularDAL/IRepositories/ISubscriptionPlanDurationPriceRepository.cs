using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    /// <summary>
    /// Repository interface for SubscriptionPlanDurationPrice entity operations.
    /// </summary>
    public interface ISubscriptionPlanDurationPriceRepository : IGenericRepository<SubscriptionPlanDurationPrice>
    {
        /// <summary>
        /// Retrieves all subscription plan duration prices with the SubscriptionPlanType and SubscriptionDurationType navigations eagerly loaded.
        /// </summary>
        Task<IEnumerable<SubscriptionPlanDurationPrice>> GetAllWithDetails();

        /// <summary>
        /// Retrieves a subscription plan duration price by id with the SubscriptionPlanType and SubscriptionDurationType navigations eagerly loaded.
        /// </summary>
        Task<SubscriptionPlanDurationPrice> GetByIdWithDetails(Guid id);
    }
}
