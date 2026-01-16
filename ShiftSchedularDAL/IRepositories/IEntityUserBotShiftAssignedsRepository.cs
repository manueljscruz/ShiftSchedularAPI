using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityUserBotShiftAssignedsRepository : IGenericRepository<EntityUserBotShiftAssigned>
    {
        Task<IEnumerable<EntityUserBotShiftAssigned>> GetAllByEntityIdAndUserBotId(Guid entityId, Guid userBotId);
        Task<bool> DeleteAllByEntityIdAndUserBotId(Guid entityId, Guid userBotId);
        Task<IEnumerable<EntityUserBotShiftAssigned>> GetByEntityIdAndShiftId(Guid entityId, Guid shiftId);
        Task<IEnumerable<EntityUserBotShiftAssigned>> GetAllByEntityId(Guid entityId);
    }
}
