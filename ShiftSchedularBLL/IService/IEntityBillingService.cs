using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Billing;

namespace ShiftSchedularBLL.IService
{
    public interface IEntityBillingService
    {
        Task<BaseResponse<BillingSummaryDTO>> GetBillingSummaryAsync(Guid entityId, string requesterId);

        Task<BaseResponse<List<SubscriptionHistoryItemDTO>>> GetSubscriptionHistoryAsync(Guid entityId, string requesterId);

        Task<BaseResponse<List<AvailablePlanDTO>>> GetAvailablePlansAsync();
    }
}
