using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.ViewModels;

namespace ShiftSchedularBLL.IService
{
    public interface IEntityService
    {
        Task<Entity> GetEntityById(Guid entityId);
        Task<IEnumerable<Entity>> GetAllEntities();
        Task<BaseResponse<Entity>> AddEntity(FormEntityDTO newEntity);
        Task<BaseResponse<bool>> UpdateEntity(FormEntityDTO entity);
        Task<BaseResponse<bool>> DeleteEntityById(Guid entityId);
        Task<List<EntityWorkerDTO>> GetEntitiesByWorkerId(string workerId);
        Task<EntityMembersViewModel> GetEntitiesMembersViewModel(BaseViewModelRequest baseViewModelRequest);
        Task<List<EntityWorkerMemberDTO>> GetEntityMembersByList(Guid entityId, List<string> workers, string lcode);
        Task<List<SkillLocalizedDTO>> GetEntitySkills(BaseViewModelRequest baseViewModelRequest);
        Task<EntityProfileViewModel> GetEntityProfileViewModel(BaseViewModelRequest entityProfileViewModelRequest);
        Task<BaseResponse<object>> AddNewEntityMember(AddNewMemberDTO newMemberDTO);
        Task<BaseResponse<bool>> UpdateEntityMember(EditMemberDTO updateEntityMemberDTO);
        Task<BaseResponse<bool>> DeleteEntityMember(DeleteMemberDTO workerMemberDTO);
    }
}
