using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    /// <summary>
    /// Repository implementation for Campaign entity operations.
    /// </summary>
    public class CampaignRepository : GenericRepository<Campaign>, ICampaignRepository
    {
        private readonly DbSet<Campaign> _dbSet;

        public CampaignRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _dbSet = context.Set<Campaign>();
        }

        public async Task<IEnumerable<Campaign>> GetAllWithDetails()
        {
            return await _dbSet
                .Include(c => c.CampaignLocalizations)
                    .ThenInclude(l => l.Localization)
                .Include(c => c.CampaignSchedulePlans)
                .OrderBy(c => c.CampaignName)
                .ToListAsync();
        }

        public async Task<Campaign> GetByIdWithDetails(Guid id)
        {
            return await _dbSet
                .Include(c => c.CampaignLocalizations)
                    .ThenInclude(l => l.Localization)
                .Include(c => c.CampaignSchedulePlans)
                .FirstOrDefaultAsync(c => c.CampaignId == id);
        }
    }
}
