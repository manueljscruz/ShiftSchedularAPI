using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IRuleTypeBusinessAspectRepository : IGenericRepository<RuleTypeBusinessAspect>
    {
        Task<List<RuleTypeBusinessAspect>> GetRuleTypeBusinessAspectsByRuleTypeId(int ruleTypeId);
    }
}
