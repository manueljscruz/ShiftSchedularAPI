using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    /// <summary>
    /// Repository implementation for SubscriptionPlanType entity operations.
    /// </summary>
    public class SubscriptionPlanTypeRepository : GenericRepository<SubscriptionPlanType>, ISubscriptionPlanTypeRepository
    {
        private readonly DbSet<SubscriptionPlanType> _dbSet;

        public SubscriptionPlanTypeRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _dbSet = context.Set<SubscriptionPlanType>();
        }

        public async Task<IEnumerable<SubscriptionPlanType>> GetAllWithLocalizations()
        {
            return await _dbSet
                .Include(spt => spt.SubscriptionPlanTypeLocalizations)
                    .ThenInclude(l => l.Localization)
                .OrderBy(spt => spt.SubscriptionPlanTypeName)
                .ToListAsync();
        }

        public async Task<SubscriptionPlanType> GetByIdWithLocalizations(int id)
        {
            return await _dbSet
                .Include(spt => spt.SubscriptionPlanTypeLocalizations)
                    .ThenInclude(l => l.Localization)
                .FirstOrDefaultAsync(spt => spt.SubscriptionPlanTypeId == id);
        }
    }
}
