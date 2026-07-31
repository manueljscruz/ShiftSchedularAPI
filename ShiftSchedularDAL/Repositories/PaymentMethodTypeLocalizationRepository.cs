using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    /// <summary>
    /// Repository implementation for PaymentMethodTypeLocalization entity operations.
    /// </summary>
    public class PaymentMethodTypeLocalizationRepository : GenericRepository<PaymentMethodTypeLocalization>, IPaymentMethodTypeLocalizationRepository
    {
        private readonly DbSet<PaymentMethodTypeLocalization> _dbSet;

        public PaymentMethodTypeLocalizationRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _dbSet = context.Set<PaymentMethodTypeLocalization>();
        }

        public async Task<IEnumerable<PaymentMethodTypeLocalization>> GetByParentId(int paymentMethodTypeId)
        {
            return await _dbSet.Where(l => l.PaymentMethodTypeId == paymentMethodTypeId).ToListAsync();
        }

        public async Task<PaymentMethodTypeLocalization> GetByParentAndLocalizationId(int paymentMethodTypeId, int localizationId)
        {
            return await _dbSet.FirstOrDefaultAsync(l => l.PaymentMethodTypeId == paymentMethodTypeId && l.LocalizationId == localizationId);
        }
    }
}
