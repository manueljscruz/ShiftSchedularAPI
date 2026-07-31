using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    /// <summary>
    /// Repository implementation for PaymentMethodTypeCountry entity operations.
    /// </summary>
    public class PaymentMethodTypeCountryRepository : GenericRepository<PaymentMethodTypeCountry>, IPaymentMethodTypeCountryRepository
    {
        private readonly DbSet<PaymentMethodTypeCountry> _dbSet;

        public PaymentMethodTypeCountryRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _dbSet = context.Set<PaymentMethodTypeCountry>();
        }

        public async Task<IEnumerable<PaymentMethodTypeCountry>> GetByParentId(int paymentMethodTypeId)
        {
            return await _dbSet.Where(c => c.PaymentMethodTypeId == paymentMethodTypeId).ToListAsync();
        }
    }
}
