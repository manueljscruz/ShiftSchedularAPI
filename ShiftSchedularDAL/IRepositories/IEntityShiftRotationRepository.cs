using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityShiftRotationRepository : IGenericRepository<EntityShiftRotation>
    {
        Task<List<int>> GetAssignedOrderNumbersByEntityId(Guid entityId);
        Task<EntityShiftRotation> GetEntityShiftRotation(Guid entityId, int orderNo, bool isLeave);
        Task<bool> DeleteEntityShiftRotation(EntityShiftRotation entityShiftRotation);
        Task<List<EntityShiftRotation>> GetEntityShiftsRotation(Guid entityId);
    }
}
