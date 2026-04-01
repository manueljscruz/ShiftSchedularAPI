using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularEntity.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShiftSchedularBLL.IService
{
    public interface IEntityService
    {
        Task<EntityDTO> GetEntityById(Guid entityId, string languageCode);
        Task<IEnumerable<Entity>> GetAllEntities();
        Task<BaseResponse<Entity>> AddEntity(FormEntityDTO newEntity);
        Task<BaseResponse<bool>> UpdateEntity(FormEntityDTO entity);
        Task<BaseResponse<bool>> DeleteEntityById(Guid entityId, string workerId);
        Task<List<EntityWorkerDTO>> GetEntitiesByWorkerId(string workerId);
        Task<EntityMembersViewModel> GetEntitiesMembersViewModel(MemberPagedModelRequestDTO memberListModelRequest);
        Task<List<SkillLocalizedDTO>> GetEntitySkills(BaseViewModelRequest baseViewModelRequest);
        Task<EntityProfileViewModel> GetEntityProfileViewModel(BaseViewModelRequest entityProfileViewModelRequest);
        Task<BaseResponse<object>> AddNewEntityMember(AddNewMemberDTO newMemberDTO);
        Task<BaseResponse<bool>> UpdateEntityMember(EditMemberDTO updateEntityMemberDTO);
        Task<BaseResponse<bool>> DeleteEntityMember(DeleteMemberDTO workerMemberDTO);
        Task<List<EntityWorkerMemberDTO>> GetEntityMembers(Guid entityId, List<string> workers);
        Task<PagedList<EntityWorkerMemberDTO>> GetEntityMembers(Guid entityId, List<string> workers, MemberListFilterDTO memberListFilterDTO, int nextPage = 0, int itemsPerPage = 0);
        Task<BaseResponse<EntityWorkerMemberDTO>> ConvertBotToUser(ConvertBotToUserDTO convertBotToUserDTO);
        Task<List<EntityDTO>> GetChildEntities(Guid parentEntityId);
        Task<BaseResponse<bool>> UpdateMemberPermission(UpdateMemberPermissionDTO dto);
        Task<List<PendingInvitationDTO>> GetPendingInvitations(string workerId);
        Task<BaseResponse<EntityWorkerDTO>> AcceptInvitation(AcceptDeclineInvitationDTO dto);
        Task<BaseResponse<bool>> DeclineInvitation(AcceptDeclineInvitationDTO dto);
        Task<List<EntityDTO>> GetUmbrellaEntities(Guid entityId);
        Task<BaseResponse<bool>> TransferCopyMembers(TransferMembersDTO dto, string requesterId);
        Task<BaseResponse<ImportCandidatesDTO>> GetImportCandidates(Guid entityId, string requesterId);
        Task<BaseResponse<int>> ImportConfigFromParent(ImportConfigDTO dto, string requesterId);
        Task<BaseResponse<bool>> SetMemberDateToExit(MemberExitDTO dto);
    }
}
