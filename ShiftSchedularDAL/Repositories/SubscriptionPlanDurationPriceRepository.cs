using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    /// <summary>
    /// Repository implementation for SubscriptionPlanDurationPrice entity operations.
    /// </summary>
    public class SubscriptionPlanDurationPriceRepository : GenericRepository<SubscriptionPlanDurationPrice>, ISubscriptionPlanDurationPriceRepository
    {
        private readonly DbSet<SubscriptionPlanDurationPrice> _dbSet;

        public SubscriptionPlanDurationPriceRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _dbSet = context.Set<SubscriptionPlanDurationPrice>();
        }

        public async Task<IEnumerable<SubscriptionPlanDurationPrice>> GetAllWithDetails()
        {
            return await _dbSet
                .Include(spdp => spdp.SubscriptionPlanType)
                .Include(spdp => spdp.SubscriptionDurationType)
                .OrderBy(spdp => spdp.SubscriptionPlanType.SubscriptionPlanTypeName)
                    .ThenBy(spdp => spdp.SubscriptionDurationType.DurationInDays)
                .ToListAsync();
        }

        public async Task<SubscriptionPlanDurationPrice> GetByIdWithDetails(Guid id)
        {
            return await _dbSet
                .Include(spdp => spdp.SubscriptionPlanType)
                .Include(spdp => spdp.SubscriptionDurationType)
                .FirstOrDefaultAsync(spdp => spdp.SubscriptionPlanDurationPriceId == id);
        }
    }
}
