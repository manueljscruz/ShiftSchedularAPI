using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityShiftRotationRepository : IGenericRepository<EntityShiftRotation>
    {
        Task<List<int>> GetAssignedOrderNumbersByEntityId(Guid entityId);
        Task<EntityShiftRotation> GetEntityShiftRotation(Guid entityId, int orderNo);
        Task<EntityShiftRotation> GetEntityShiftRotation(Guid entityId, Guid shiftId);
        Task<bool> DeleteEntityShiftRotation(EntityShiftRotation entityShiftRotation);
        Task<List<EntityShiftRotation>> GetEntityShiftsRotation(Guid entityId);
    }
}
