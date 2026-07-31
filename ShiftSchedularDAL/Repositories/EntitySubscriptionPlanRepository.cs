using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    /// <summary>
    /// Repository implementation for EntitySubscriptionPlan entity operations.
    /// </summary>
    public class EntitySubscriptionPlanRepository : GenericRepository<EntitySubscriptionPlan>, IEntitySubscriptionPlanRepository
    {
        private readonly DbSet<EntitySubscriptionPlan> _dbSet;

        public EntitySubscriptionPlanRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _dbSet = context.Set<EntitySubscriptionPlan>();
        }

        public async Task<EntitySubscriptionPlan> GetActiveByEntityId(Guid entityId)
        {
            return await _dbSet
                .Include(x => x.SubscriptionPlanDurationPrice)
                    .ThenInclude(x => x.SubscriptionPlanType)
                .Include(x => x.SubscriptionPlanDurationPrice)
                    .ThenInclude(x => x.SubscriptionDurationType)
                .Where(x => x.EntityId.Equals(entityId) && x.Status == "Active")
                .OrderByDescending(x => x.StartDate)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<EntitySubscriptionPlan>> GetHistoryByEntityId(Guid entityId)
        {
            return await _dbSet
                .Include(x => x.SubscriptionPlanDurationPrice)
                    .ThenInclude(x => x.SubscriptionPlanType)
                .Include(x => x.SubscriptionPlanDurationPrice)
                    .ThenInclude(x => x.SubscriptionDurationType)
                .Where(x => x.EntityId.Equals(entityId))
                .OrderByDescending(x => x.StartDate)
                .ToListAsync();
        }
    }
}
