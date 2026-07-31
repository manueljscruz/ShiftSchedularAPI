using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    /// <summary>
    /// Repository interface for PaymentMethodTypeLocalization entity operations.
    /// </summary>
    public interface IPaymentMethodTypeLocalizationRepository : IGenericRepository<PaymentMethodTypeLocalization>
    {
        /// <summary>
        /// Retrieves all localization rows for a given payment method type.
        /// </summary>
        Task<IEnumerable<PaymentMethodTypeLocalization>> GetByParentId(int paymentMethodTypeId);

        /// <summary>
        /// Retrieves a single localization row for a given payment method type and localization.
        /// </summary>
        Task<PaymentMethodTypeLocalization> GetByParentAndLocalizationId(int paymentMethodTypeId, int localizationId);
    }
}
