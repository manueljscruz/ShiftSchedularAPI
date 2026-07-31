using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    /// <summary>
    /// Repository implementation for CampaignSubscriptionPlan entity operations.
    /// </summary>
    public class CampaignSubscriptionPlanRepository : GenericRepository<CampaignSubscriptionPlan>, ICampaignSubscriptionPlanRepository
    {
        private readonly DbSet<CampaignSubscriptionPlan> _dbSet;

        public CampaignSubscriptionPlanRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _dbSet = context.Set<CampaignSubscriptionPlan>();
        }

        public async Task<IEnumerable<CampaignSubscriptionPlan>> GetByParentId(Guid campaignId)
        {
            return await _dbSet.Where(csp => csp.CampaignId == campaignId).ToListAsync();
        }
    }
}
