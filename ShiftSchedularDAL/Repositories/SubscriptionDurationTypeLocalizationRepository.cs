using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    /// <summary>
    /// Repository implementation for SubscriptionDurationTypeLocalization entity operations.
    /// </summary>
    public class SubscriptionDurationTypeLocalizationRepository : GenericRepository<SubscriptionDurationTypeLocalization>, ISubscriptionDurationTypeLocalizationRepository
    {
        private readonly DbSet<SubscriptionDurationTypeLocalization> _dbSet;

        public SubscriptionDurationTypeLocalizationRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _dbSet = context.Set<SubscriptionDurationTypeLocalization>();
        }

        public async Task<IEnumerable<SubscriptionDurationTypeLocalization>> GetByParentId(int subscriptionDurationTypeId)
        {
            return await _dbSet.Where(l => l.SubscriptionDurationTypeId == subscriptionDurationTypeId).ToListAsync();
        }

        public async Task<SubscriptionDurationTypeLocalization> GetByParentAndLocalizationId(int subscriptionDurationTypeId, int localizationId)
        {
            return await _dbSet.FirstOrDefaultAsync(l => l.SubscriptionDurationTypeId == subscriptionDurationTypeId && l.LocalizationId == localizationId);
        }
    }
}
