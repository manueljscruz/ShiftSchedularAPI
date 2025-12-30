using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.ViewModels;

namespace ShiftSchedularBLL.IService
{
    public interface IEntityDashboardService
    {
        Task<DashboardEntityViewModel> GetEntityDashboardViewModel(BaseViewModelRequest request);
    }
}
