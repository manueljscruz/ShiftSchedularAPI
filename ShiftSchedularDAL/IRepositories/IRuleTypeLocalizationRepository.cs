using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IRuleTypeLocalizationRepository : IGenericRepository<RuleTypeLocalization>
    {
        IEnumerable<RuleTypeLocalization> GetRuleTypeLocalizationsByRuleId(int ruleTypeId);
        Task<IEnumerable<RuleTypeLocalization>> GetRuleTypesByLocalization(string languageCode);
    }
}
