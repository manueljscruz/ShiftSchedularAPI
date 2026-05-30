using ShiftSchedularEntity.Models.DataTransferObjects.Admin;
using ShiftSchedularEntity.Models.ViewModels;

namespace ShiftSchedularBLL.IService
{
    public interface IHomeService
    {
        Task<HomeViewModel> GetHomeViewModel(string localizationCode);
        Task SendEmailTest(string email);
        Task<AdminDashboardViewModel> GetAdminDashboardViewModel();
    }
}
