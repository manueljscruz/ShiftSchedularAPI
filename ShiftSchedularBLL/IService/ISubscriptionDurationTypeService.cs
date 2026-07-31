using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Admin;

namespace ShiftSchedularBLL.IService
{
    /// <summary>
    /// Service interface for SubscriptionDurationType admin management.
    /// </summary>
    public interface ISubscriptionDurationTypeService
    {
        Task<List<AdminSubscriptionDurationTypeItemDTO>> GetAllAsync();
        Task<AdminSubscriptionDurationTypeItemDTO> GetByIdAsync(int id);
        Task<BaseResponse<int>> UpsertAsync(AdminUpsertSubscriptionDurationTypeDTO dto);
        Task<BaseResponse<bool>> DeleteAsync(int id);
    }
}
