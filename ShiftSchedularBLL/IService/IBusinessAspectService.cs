using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.APIManagement;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularBLL.IService
{
    public interface IBusinessAspectService
    {
        Task<int> AddBusinessAspect(string strNewBusinessAspect);
        Task<bool> AddBusinessAspectLocalization(BusinessAspectLocalizationSubmissionModel submissionModel);
        Task DeleteBusinessAspectById(int id);
        Task<IEnumerable<BusinessAspect>> GetAllBusinessAspects();
        Task<List<BusinessAspectLocalizedDTO>> GetAllBusinessAspectsByLocalization(string lcode);
        Task<BusinessAspect> GetBusinessAspectById(int id);
        Task UpdateBusinessAspect(BusinessAspect businessAspect);
    }
}
