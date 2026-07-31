using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Admin;

namespace ShiftSchedularBLL.IService
{
    /// <summary>
    /// Service interface for PaymentMethodType admin management.
    /// </summary>
    public interface IPaymentMethodTypeService
    {
        Task<List<AdminPaymentMethodTypeItemDTO>> GetAllAsync();
        Task<AdminPaymentMethodTypeItemDTO> GetByIdAsync(int id);
        Task<BaseResponse<int>> UpsertAsync(AdminUpsertPaymentMethodTypeDTO dto);
        Task<BaseResponse<bool>> DeleteAsync(int id);
    }
}
