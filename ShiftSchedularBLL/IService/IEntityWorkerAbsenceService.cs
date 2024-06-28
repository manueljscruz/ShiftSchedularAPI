using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularEntity.Models.ViewModels;

namespace ShiftSchedularBLL.IService
{
    public interface IEntityWorkerAbsenceService
    {
        Task<BaseResponse<bool>> AbsenceApprovalDecision(AbsenceApprovalDecisionDTO absenceApprovalDecisionDTO);
        Task<BaseResponse<EntityWorkerAbsenceDTO>> AddEntityWorkerAbsence(AddEntityWorkerAbsenceDTO addEntityWorkerAbsence);
        Task<BaseResponse<bool>> DeleteEntityWorkerAbsence(string absenceId);
        Task<EntityWorkerAbsenceDTO> GetEntityWorkerAbsenceById(string id, string lcode);
        Task<EntityWorkerAbsenceViewModel> GetEntityWorkerAbsenceViewModel(BaseViewModelRequest viewModelRequestDTO);
        Task<BaseResponse<bool>> UpdateEntityWorkerAbsence(EntityWorkerAbsenceDTO entityWorkerAbsenceDTO);
    }
}
