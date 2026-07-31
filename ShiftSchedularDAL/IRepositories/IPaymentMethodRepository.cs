using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    /// <summary>
    /// Repository interface for PaymentMethod entity operations.
    /// </summary>
    public interface IPaymentMethodRepository : IGenericRepository<PaymentMethod>
    {
        /// <summary>
        /// Retrieves all payment methods for an entity, with the payment method type eagerly loaded.
        /// </summary>
        Task<IEnumerable<PaymentMethod>> GetByEntityId(Guid entityId);
    }
}
