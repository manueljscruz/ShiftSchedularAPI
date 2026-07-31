using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    /// <summary>
    /// Repository interface for SubscriptionPlanTypeLocalization entity operations.
    /// </summary>
    public interface ISubscriptionPlanTypeLocalizationRepository : IGenericRepository<SubscriptionPlanTypeLocalization>
    {
        /// <summary>
        /// Retrieves all localization rows for a given subscription plan type.
        /// </summary>
        Task<IEnumerable<SubscriptionPlanTypeLocalization>> GetByParentId(int subscriptionPlanTypeId);

        /// <summary>
        /// Retrieves a single localization row for a given subscription plan type and localization.
        /// </summary>
        Task<SubscriptionPlanTypeLocalization> GetByParentAndLocalizationId(int subscriptionPlanTypeId, int localizationId);
    }
}
