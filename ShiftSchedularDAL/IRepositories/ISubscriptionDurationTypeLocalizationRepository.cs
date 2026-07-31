using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    /// <summary>
    /// Repository interface for SubscriptionDurationTypeLocalization entity operations.
    /// </summary>
    public interface ISubscriptionDurationTypeLocalizationRepository : IGenericRepository<SubscriptionDurationTypeLocalization>
    {
        /// <summary>
        /// Retrieves all localization rows for a given subscription duration type.
        /// </summary>
        Task<IEnumerable<SubscriptionDurationTypeLocalization>> GetByParentId(int subscriptionDurationTypeId);

        /// <summary>
        /// Retrieves a single localization row for a given subscription duration type and localization.
        /// </summary>
        Task<SubscriptionDurationTypeLocalization> GetByParentAndLocalizationId(int subscriptionDurationTypeId, int localizationId);
    }
}
