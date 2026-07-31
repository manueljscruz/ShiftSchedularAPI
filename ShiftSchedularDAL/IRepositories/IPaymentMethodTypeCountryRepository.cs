using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    /// <summary>
    /// Repository interface for PaymentMethodTypeCountry entity operations.
    /// </summary>
    public interface IPaymentMethodTypeCountryRepository : IGenericRepository<PaymentMethodTypeCountry>
    {
        /// <summary>
        /// Retrieves all country availability rows for a given payment method type.
        /// </summary>
        Task<IEnumerable<PaymentMethodTypeCountry>> GetByParentId(int paymentMethodTypeId);
    }
}
