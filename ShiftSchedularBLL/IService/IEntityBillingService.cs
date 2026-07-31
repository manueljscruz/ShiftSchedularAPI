using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Billing;

namespace ShiftSchedularBLL.IService
{
    public interface IEntityBillingService
    {
        Task<BaseResponse<BillingSummaryDTO>> GetBillingSummaryAsync(Guid entityId, string requesterId);

        Task<BaseResponse<List<SubscriptionHistoryItemDTO>>> GetSubscriptionHistoryAsync(Guid entityId, string requesterId);

        Task<BaseResponse<List<AvailablePlanDTO>>> GetAvailablePlansAsync();

        Task<BaseResponse<SetupIntentDTO>> CreateSetupIntentAsync(Guid entityId, string requesterId);

        Task<BaseResponse<PaymentMethodSummaryDTO>> ConfirmPaymentMethodAsync(Guid entityId, string requesterId, ConfirmPaymentMethodDTO dto);

        Task<BaseResponse<bool>> SetDefaultPaymentMethodAsync(Guid entityId, string requesterId, Guid paymentMethodId);

        Task<BaseResponse<bool>> RemovePaymentMethodAsync(Guid entityId, string requesterId, Guid paymentMethodId);

        Task<BaseResponse<Guid>> SubscribeAsync(Guid entityId, string requesterId, SubscribeRequestDTO dto);
    }
}
