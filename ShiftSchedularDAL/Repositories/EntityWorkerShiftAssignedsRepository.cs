using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class EntityWorkerShiftAssignedsRepository : GenericRepository<EntityWorkerShiftAssigned>, IEntityWorkerShiftAssignedsRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<EntityWorkerShiftAssigned> _entityWorkerShiftAssignedDbSet;
        private readonly IUnitOfWork _unitOfWork;
        public EntityWorkerShiftAssignedsRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _entityWorkerShiftAssignedDbSet = _context.Set<EntityWorkerShiftAssigned>();
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> DeleteAllByEntityIdAndUserId(Guid entityId, Guid workerGuid)
        {
            if (entityId != Guid.Empty && workerGuid != Guid.Empty)
            {
                IEnumerable<EntityWorkerShiftAssigned> workerShiftAssigneds = await GetAllByEntityIdAndUserId(entityId, workerGuid);
                _entityWorkerShiftAssignedDbSet.RemoveRange(workerShiftAssigneds);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            else return false;
        }

        public async Task<IEnumerable<EntityWorkerShiftAssigned>> GetAllByEntityIdAndUserId(Guid entityId, Guid userId)
        {
            if (entityId != Guid.Empty && userId != Guid.Empty)
            {
                return await _entityWorkerShiftAssignedDbSet.Where(i => i.EntityId.Equals(entityId) && i.ApplicationUserId.Equals(userId)).ToListAsync();
            }
            else
                return null;
        }

        public async Task<IEnumerable<EntityWorkerShiftAssigned>> GetByEntityIdAndShiftId(Guid entityId, Guid shiftId)
        {
            if (entityId != Guid.Empty && shiftId != Guid.Empty)
            {
                return await _entityWorkerShiftAssignedDbSet.Where(i => i.EntityId.Equals(entityId) && i.ShiftId.Equals(shiftId)).ToListAsync();
            }
            else
                return null;
        }

        public async Task<IEnumerable<EntityWorkerShiftAssigned>> GetAllByEntityId(Guid entityId)
        {
            if (entityId != Guid.Empty)
            {
                return await _entityWorkerShiftAssignedDbSet.Where(i => i.EntityId.Equals(entityId)).ToListAsync();
            }
            else
                return null;
        }
    }
}
