using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    /// <summary>
    /// Repository interface for CampaignLocalization entity operations.
    /// </summary>
    public interface ICampaignLocalizationRepository : IGenericRepository<CampaignLocalization>
    {
        /// <summary>
        /// Retrieves all localization rows for a given campaign.
        /// </summary>
        Task<IEnumerable<CampaignLocalization>> GetByParentId(Guid campaignId);

        /// <summary>
        /// Retrieves a single localization row for a given campaign and localization.
        /// </summary>
        Task<CampaignLocalization> GetByParentAndLocalizationId(Guid campaignId, int localizationId);
    }
}
