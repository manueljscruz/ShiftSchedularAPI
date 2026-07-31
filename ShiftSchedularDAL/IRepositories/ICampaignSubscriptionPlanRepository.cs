using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    /// <summary>
    /// Repository interface for CampaignSubscriptionPlan entity operations.
    /// </summary>
    public interface ICampaignSubscriptionPlanRepository : IGenericRepository<CampaignSubscriptionPlan>
    {
        /// <summary>
        /// Retrieves all eligible subscription plan duration price associations for a given campaign.
        /// </summary>
        Task<IEnumerable<CampaignSubscriptionPlan>> GetByParentId(Guid campaignId);
    }
}
