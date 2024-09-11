using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.APIManagement;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularBLL.IService
{
    public interface IBaseEntityRuleService
    {
        Task<List<BaseEntityRuleDTO>> GetBaseEntityRules(string lcode);
        Task<List<EntityRuleDTO>> GetBaseEntityRulesAsEntityRules(string lcode);
        Task<BaseEntityRule> GetBaseEntityRuleById(int id);
        Task<int> AddBaseEntityRule(BaseEntityRuleSubmissionModel baseEntityRuleSubmissionModel);
        Task<bool> AddBaseEntityRuleSpecification(BaseEntityRuleSpecificationSubmissionModel baseEntityRuleSpecificationSubmissionModel);
        Task<bool> DeleteBaseEntityRuleById(int id);
        Task<bool> DeleteBaseEntityRuleSpecification(int baseRuleId, int specId);
        Task<bool> UpdateBaseEntityRule(BaseEntityRule baseEntityRule);
        Task<bool> UpdateBaseEntityRuleSpecification(BaseEntityRuleSpecification baseEntityRuleSpecification);
    }
}
