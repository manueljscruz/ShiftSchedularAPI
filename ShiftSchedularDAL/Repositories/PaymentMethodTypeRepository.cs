using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    /// <summary>
    /// Repository implementation for PaymentMethodType entity operations.
    /// </summary>
    public class PaymentMethodTypeRepository : GenericRepository<PaymentMethodType>, IPaymentMethodTypeRepository
    {
        private readonly DbSet<PaymentMethodType> _dbSet;

        public PaymentMethodTypeRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _dbSet = context.Set<PaymentMethodType>();
        }

        public async Task<IEnumerable<PaymentMethodType>> GetAllWithDetails()
        {
            return await _dbSet
                .Include(pmt => pmt.PaymentMethodTypeCountries)
                .Include(pmt => pmt.PaymentMethodTypeLocalizations)
                    .ThenInclude(l => l.Localization)
                .OrderBy(pmt => pmt.PaymentMethodTypeName)
                .ToListAsync();
        }

        public async Task<PaymentMethodType> GetByIdWithDetails(int id)
        {
            return await _dbSet
                .Include(pmt => pmt.PaymentMethodTypeCountries)
                .Include(pmt => pmt.PaymentMethodTypeLocalizations)
                    .ThenInclude(l => l.Localization)
                .FirstOrDefaultAsync(pmt => pmt.PaymentMethodTypeId == id);
        }
    }
}
