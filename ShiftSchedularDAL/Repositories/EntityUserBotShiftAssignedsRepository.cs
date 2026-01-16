using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class EntityUserBotShiftAssignedsRepository : GenericRepository<EntityUserBotShiftAssigned>, IEntityUserBotShiftAssignedsRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<EntityUserBotShiftAssigned> _entityUserBotShiftAssignedDbSet;
        private readonly IUnitOfWork _unitOfWork;

        public EntityUserBotShiftAssignedsRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _entityUserBotShiftAssignedDbSet = _context.Set<EntityUserBotShiftAssigned>();
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> DeleteAllByEntityIdAndUserBotId(Guid entityId, Guid userBotId)
        {
            if (entityId != Guid.Empty && userBotId != Guid.Empty)
            {
                IEnumerable<EntityUserBotShiftAssigned> entityUserBotShiftAssigneds = await GetAllByEntityIdAndUserBotId(entityId, userBotId);
                _entityUserBotShiftAssignedDbSet.RemoveRange(entityUserBotShiftAssigneds);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            else return false;
        }

        public async Task<IEnumerable<EntityUserBotShiftAssigned>> GetAllByEntityIdAndUserBotId(Guid entityId, Guid userId)
        {
            if (entityId != Guid.Empty && userId != Guid.Empty)
            {
                return await _entityUserBotShiftAssignedDbSet.Where(i => i.EntityId.Equals(entityId) && i.UserBotId.Equals(userId)).ToListAsync();
            }
            else
                return null; 
        }

        public async Task<IEnumerable<EntityUserBotShiftAssigned>> GetByEntityIdAndShiftId(Guid entityId, Guid shiftId)
        {
            if (entityId != Guid.Empty && shiftId != Guid.Empty)
            {
                return await _entityUserBotShiftAssignedDbSet.Where(i => i.EntityId.Equals(entityId) && i.ShiftId.Equals(shiftId)).ToListAsync();
            }
            else
                return null;
        }

        public async Task<IEnumerable<EntityUserBotShiftAssigned>> GetAllByEntityId(Guid entityId)
        {
            if (entityId != Guid.Empty)
            {
                return await _entityUserBotShiftAssignedDbSet.Where(i => i.EntityId.Equals(entityId)).ToListAsync();
            }
            else
                return null;
        }
    }
}
