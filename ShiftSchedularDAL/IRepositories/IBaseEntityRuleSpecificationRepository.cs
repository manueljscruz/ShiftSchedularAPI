using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IBaseEntityRuleSpecificationRepository
    {
        Task<BaseEntityRuleSpecification> AddBaseEntityRuleSpecification(BaseEntityRuleSpecification baseEntityRuleSpecification);
        Task<List<BaseEntityRuleSpecification>> GetRuleSpecificationsById(int ruleSpecId);
        Task<bool> DeleteBaseEntityRuleSpecification(int baseRuleId, int specId);
        Task<bool> DeleteAllBaseEntityRuleSpecificationsById(int baseRuleId);
        Task<bool> UpdateBaseEntityRuleSpecification(BaseEntityRuleSpecification baseEntityRuleSpecification);
    }
}
