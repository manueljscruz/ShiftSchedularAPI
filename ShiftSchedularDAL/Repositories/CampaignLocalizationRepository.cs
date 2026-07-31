using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    /// <summary>
    /// Repository implementation for CampaignLocalization entity operations.
    /// </summary>
    public class CampaignLocalizationRepository : GenericRepository<CampaignLocalization>, ICampaignLocalizationRepository
    {
        private readonly DbSet<CampaignLocalization> _dbSet;

        public CampaignLocalizationRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _dbSet = context.Set<CampaignLocalization>();
        }

        public async Task<IEnumerable<CampaignLocalization>> GetByParentId(Guid campaignId)
        {
            return await _dbSet.Where(l => l.CampaignId == campaignId).ToListAsync();
        }

        public async Task<CampaignLocalization> GetByParentAndLocalizationId(Guid campaignId, int localizationId)
        {
            return await _dbSet.FirstOrDefaultAsync(l => l.CampaignId == campaignId && l.LocalizationId == localizationId);
        }
    }
}
