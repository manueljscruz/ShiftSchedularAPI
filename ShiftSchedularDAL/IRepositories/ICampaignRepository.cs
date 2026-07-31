using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    /// <summary>
    /// Repository interface for Campaign entity operations.
    /// </summary>
    public interface ICampaignRepository : IGenericRepository<Campaign>
    {
        /// <summary>
        /// Retrieves all campaigns with localizations and eligible subscription plan duration prices eagerly loaded.
        /// </summary>
        Task<IEnumerable<Campaign>> GetAllWithDetails();

        /// <summary>
        /// Retrieves a campaign by id with localizations and eligible subscription plan duration prices eagerly loaded.
        /// </summary>
        Task<Campaign> GetByIdWithDetails(Guid id);
    }
}
