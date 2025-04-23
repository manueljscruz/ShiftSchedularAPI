using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularEntity.Models.ViewModels;

namespace ShiftSchedularBLL.IService
{
    public interface IEntityWorkerAbsenceService
    {
        Task<BaseResponse<EntityWorkerAbsenceDTO>> AbsenceApprovalDecision(AbsenceApprovalDecisionDTO absenceApprovalDecisionDTO);
        Task<BaseResponse<EntityWorkerAbsenceDTO>> AddEntityWorkerAbsence(AddEntityWorkerAbsenceDTO addEntityWorkerAbsence);
        Task<BaseResponse<bool>> DeleteEntityWorkerAbsence(Guid absenceId);
        Task<EntityWorkerAbsenceDTO> GetEntityWorkerAbsenceById(Guid id, string lcode);
        Task<EntityWorkerAbsenceViewModel> GetEntityWorkerAbsenceViewModel(BaseViewModelRequest viewModelRequestDTO);
        Task<BaseResponse<EntityWorkerAbsenceDTO>> UpdateEntityWorkerAbsence(EntityWorkerAbsenceDTO entityWorkerAbsenceDTO);
    }
}
