using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Admin;

namespace ShiftSchedularBLL.IService
{
    /// <summary>
    /// Service interface for SubscriptionPlanType admin management.
    /// </summary>
    public interface ISubscriptionPlanTypeService
    {
        Task<List<AdminSubscriptionPlanTypeItemDTO>> GetAllAsync();
        Task<AdminSubscriptionPlanTypeItemDTO> GetByIdAsync(int id);
        Task<BaseResponse<int>> UpsertAsync(AdminUpsertSubscriptionPlanTypeDTO dto);
        Task<BaseResponse<bool>> DeleteAsync(int id);
    }
}
