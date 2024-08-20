using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IBaseEntityRuleRepository : IGenericRepository<BaseEntityRule>
    {
        Task<IEnumerable<BaseEntityRule>> GetBaseEntityRules();
    }
}
