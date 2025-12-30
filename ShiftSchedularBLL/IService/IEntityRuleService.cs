using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularEntity.Models.ViewModels;

namespace ShiftSchedularBLL.IService
{
    public interface IEntityRuleService
    {
        Task<BaseResponse<EntityRuleDTO>> AddEntityRule(AddEntityRuleDTO addEntityRuleDTO);
        Task<BaseResponse<EntityRuleSpecificationDTO>> AddEntityRuleSpecification(AddEntityRuleSpecificationDTO addEntityRuleSpecificationDTO);
        Task<BaseResponse<bool>> DeleteEntityRule(Guid entityId, Guid entityRuleId);
        Task<BaseResponse<bool>> DeleteEntityRuleSpecification(Guid entityRuleId, int specificationId);
        Task<EntityRuleViewModel> GetEntityRuleViewModel(BaseViewModelRequest entityRuleViewModelRequestDTO);
        Task<List<EntityRuleDTO>> GetEntityRules(Guid entityId, string lcode);
        Task<EntityRuleDTO> GetEntityRuleById(Guid entityRuleId, string lcode);
        Task<BaseResponse<bool>> UpdateEntityRule(EntityRuleDTO entityRule);
        Task<BaseResponse<bool>> UpdateEntityRuleSpecification(EntityRuleSpecificationDTO entityRuleSpecification);
        Task<BaseResponse<bool>> DeleteEntityRuleSpecifications(Guid entityRuleId);
        Task<List<EntityRuleDTO>> GetSpecificRules(Guid entityId, List<string> filteredRules, string languageCode);
        Task<int> GetTotalEntityRules(Guid entityId);
    }
}
