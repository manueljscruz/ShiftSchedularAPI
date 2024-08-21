using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IBaseEntityRuleRepository : IGenericRepository<BaseEntityRule>
    {
        Task<List<BaseEntityRule>> GetBaseEntityRules();
    }
}
