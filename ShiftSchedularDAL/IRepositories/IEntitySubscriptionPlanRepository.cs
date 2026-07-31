using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    /// <summary>
    /// Repository interface for EntitySubscriptionPlan entity operations.
    /// </summary>
    public interface IEntitySubscriptionPlanRepository : IGenericRepository<EntitySubscriptionPlan>
    {
        /// <summary>
        /// Retrieves the active subscription plan for an entity, with plan type and duration type eagerly loaded.
        /// </summary>
        Task<EntitySubscriptionPlan> GetActiveByEntityId(Guid entityId);

        /// <summary>
        /// Retrieves the full subscription history for an entity, ordered by start date descending, with plan type and duration type eagerly loaded.
        /// </summary>
        Task<IEnumerable<EntitySubscriptionPlan>> GetHistoryByEntityId(Guid entityId);
    }
}
