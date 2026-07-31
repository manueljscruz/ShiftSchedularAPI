using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Admin;

namespace ShiftSchedularBLL.IService
{
    /// <summary>
    /// Service interface for Campaign admin management.
    /// </summary>
    public interface ICampaignService
    {
        Task<List<AdminCampaignItemDTO>> GetAllAsync();
        Task<AdminCampaignItemDTO> GetByIdAsync(Guid id);
        Task<BaseResponse<Guid>> UpsertAsync(AdminUpsertCampaignDTO dto);
        Task<BaseResponse<bool>> DeleteAsync(Guid id);
    }
}
