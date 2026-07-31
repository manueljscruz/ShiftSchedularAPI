using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Admin;

namespace ShiftSchedularBLL.IService
{
    /// <summary>
    /// Service interface for SubscriptionPlanDurationPrice admin management.
    /// </summary>
    public interface ISubscriptionPlanDurationPriceService
    {
        Task<List<AdminSubscriptionPlanDurationPriceItemDTO>> GetAllAsync();
        Task<AdminSubscriptionPlanDurationPriceItemDTO> GetByIdAsync(Guid id);
        Task<BaseResponse<Guid>> UpsertAsync(AdminUpsertSubscriptionPlanDurationPriceDTO dto);
        Task<BaseResponse<bool>> DeleteAsync(Guid id);
    }
}
