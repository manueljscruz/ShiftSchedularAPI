using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    /// <summary>
    /// Repository implementation for PaymentMethod entity operations.
    /// </summary>
    public class PaymentMethodRepository : GenericRepository<PaymentMethod>, IPaymentMethodRepository
    {
        private readonly DbSet<PaymentMethod> _dbSet;

        public PaymentMethodRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _dbSet = context.Set<PaymentMethod>();
        }

        public async Task<IEnumerable<PaymentMethod>> GetByEntityId(Guid entityId)
        {
            return await _dbSet
                .Include(x => x.PaymentMethodType)
                .Where(x => x.EntityId.Equals(entityId))
                .OrderByDescending(x => x.IsDefault)
                .ToListAsync();
        }
    }
}
