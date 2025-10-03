using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityRuleRepository : IGenericRepository<EntityRule>
    {
        Task<EntityRule> GetById(Guid entityRuleId);
        Task<IEnumerable<EntityRule>> GetEntityRules(Guid entityId);
    }
}
