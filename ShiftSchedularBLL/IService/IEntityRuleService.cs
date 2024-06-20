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
        Task<BaseResponse<bool>> DeleteEntityRule(string entityId, string entityRuleId);
        Task<BaseResponse<bool>> DeleteEntityRuleSpecification(string entityRuleId, int specificationId);
        Task<EntityRuleViewModel> GetEntityRuleViewModel(BaseViewModelRequest entityRuleViewModelRequestDTO);
        Task<EntityRuleDTO> GetEntityRuleById(string entityRuleId, string lcode);
        Task<BaseResponse<bool>> UpdateEntityRule(EntityRuleDTO entityRule);
        Task<BaseResponse<bool>> UpdateEntityRuleSpecification(EntityRuleSpecificationDTO entityRuleSpecification);
        Task<BaseResponse<bool>> DeleteEntityRuleSpecifications(string entityRuleId);
    }
}
