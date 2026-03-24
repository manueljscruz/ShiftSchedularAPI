using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityWorkerSkillRepository : IGenericRepository<EntityWorkerSkill>
    {
        Task<IEnumerable<EntityWorkerSkill>> GetByEntityId(Guid entityId);
        Task<bool> DeleteAllByEntityIdAndUserId(Guid entityId, Guid userId);
        Task<EntityWorkerSkill?> FindByWorkerEntityAndSkill(string workerId, Guid entityId, int skillId);
    }
}
