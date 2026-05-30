using ShiftSchedularEntity.Models.DataTransferObjects.Admin;

namespace ShiftSchedularBLL.Admin
{
    public interface IAdminTypeHandler
    {
        bool HasColors { get; }
        Task<List<AdminTypeItemDTO>> GetAllAsync();
        Task<AdminTypeItemDTO?> GetByIdAsync(int id);
        Task<int> UpsertAsync(AdminUpsertTypeDTO dto);
        Task DeleteAsync(int id);
    }
}
