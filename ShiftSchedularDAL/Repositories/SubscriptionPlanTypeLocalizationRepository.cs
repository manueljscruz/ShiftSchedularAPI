using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    /// <summary>
    /// Repository implementation for SubscriptionPlanTypeLocalization entity operations.
    /// </summary>
    public class SubscriptionPlanTypeLocalizationRepository : GenericRepository<SubscriptionPlanTypeLocalization>, ISubscriptionPlanTypeLocalizationRepository
    {
        private readonly DbSet<SubscriptionPlanTypeLocalization> _dbSet;

        public SubscriptionPlanTypeLocalizationRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _dbSet = context.Set<SubscriptionPlanTypeLocalization>();
        }

        public async Task<IEnumerable<SubscriptionPlanTypeLocalization>> GetByParentId(int subscriptionPlanTypeId)
        {
            return await _dbSet.Where(l => l.SubscriptionPlanTypeId == subscriptionPlanTypeId).ToListAsync();
        }

        public async Task<SubscriptionPlanTypeLocalization> GetByParentAndLocalizationId(int subscriptionPlanTypeId, int localizationId)
        {
            return await _dbSet.FirstOrDefaultAsync(l => l.SubscriptionPlanTypeId == subscriptionPlanTypeId && l.LocalizationId == localizationId);
        }
    }
}
