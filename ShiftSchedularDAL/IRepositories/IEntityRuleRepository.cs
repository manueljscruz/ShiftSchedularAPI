using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityRuleRepository : IGenericRepository<EntityRule>
    {
        Task<IEnumerable<EntityRule>> GetEntityRules(string entityId);
    }
}
