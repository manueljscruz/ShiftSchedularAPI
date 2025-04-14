using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityUserBotSkillRepository : IGenericRepository<EntityUserBotSkill>
    {
        /// <summary>
        /// Gets the skills of all user bot for a given entity
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        Task<IEnumerable<EntityUserBotSkill>> GetByEntityId(Guid entityId);

        Task<IEnumerable<EntityUserBotSkill>> GetByEntityIdAndUserBotId(Guid entityId, Guid userBotId);

        Task<bool> DeleteAllByEntityIdAndUserBotId(Guid entityId, Guid userBotId);
    }
}
