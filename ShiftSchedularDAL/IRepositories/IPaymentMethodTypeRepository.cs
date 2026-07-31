using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    /// <summary>
    /// Repository interface for PaymentMethodType entity operations.
    /// </summary>
    public interface IPaymentMethodTypeRepository : IGenericRepository<PaymentMethodType>
    {
        /// <summary>
        /// Retrieves all payment method types with their localizations and available countries eagerly loaded.
        /// </summary>
        Task<IEnumerable<PaymentMethodType>> GetAllWithDetails();

        /// <summary>
        /// Retrieves a payment method type by id with its localizations and available countries eagerly loaded.
        /// </summary>
        Task<PaymentMethodType> GetByIdWithDetails(int id);
    }
}
