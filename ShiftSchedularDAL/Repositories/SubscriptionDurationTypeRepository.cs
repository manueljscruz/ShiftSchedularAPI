using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    /// <summary>
    /// Repository implementation for SubscriptionDurationType entity operations.
    /// </summary>
    public class SubscriptionDurationTypeRepository : GenericRepository<SubscriptionDurationType>, ISubscriptionDurationTypeRepository
    {
        private readonly DbSet<SubscriptionDurationType> _dbSet;

        public SubscriptionDurationTypeRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _dbSet = context.Set<SubscriptionDurationType>();
        }

        public async Task<IEnumerable<SubscriptionDurationType>> GetAllWithLocalizations()
        {
            return await _dbSet
                .Include(sdt => sdt.SubscriptionDurationTypeLocalizations)
                    .ThenInclude(l => l.Localization)
                .OrderBy(sdt => sdt.DurationInDays)
                .ToListAsync();
        }

        public async Task<SubscriptionDurationType> GetByIdWithLocalizations(int id)
        {
            return await _dbSet
                .Include(sdt => sdt.SubscriptionDurationTypeLocalizations)
                    .ThenInclude(l => l.Localization)
                .FirstOrDefaultAsync(sdt => sdt.SubscriptionDurationTypeId == id);
        }
    }
}
