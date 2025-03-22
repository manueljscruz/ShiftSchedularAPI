using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class EntityShiftRotationRepository : GenericRepository<EntityShiftRotation>, IEntityShiftRotationRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<EntityShiftRotation> _dbSet;
        private readonly IUnitOfWork _unitOfWork;

        public EntityShiftRotationRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _dbSet = _context.Set<EntityShiftRotation>();
        }

        public async Task<List<int>> GetAssignedOrderNumbersByEntityId(Guid entityId)
        {
            if (entityId != Guid.Empty)
            {
                return await _dbSet.Where(i => i.EntityId.Equals(entityId)).Select(j => j.OrderNo).ToListAsync();
            }
            else return null;
        }

        public async Task<EntityShiftRotation> GetEntityShiftRotation(Guid entityId, int orderNo, bool isLeave)
        {
            if(entityId != Guid.Empty)
            {
                return await _dbSet.Include(x => x.Shift).FirstOrDefaultAsync(i => i.EntityId.Equals(entityId) && i.OrderNo.Equals(orderNo) && i.IsLeave.Equals(isLeave));
            }
            else return null;
        }

        public Task<List<EntityShiftRotation>> GetEntityShiftsRotation(Guid entityId)
        {
            if(entityId != Guid.Empty)
            {
                return _dbSet.Where(i => i.EntityId.Equals(entityId)).Include(x => x.Shift).ToListAsync();
            }
            else return null;
        }


        public async Task<bool> DeleteEntityShiftRotation(EntityShiftRotation entityShiftRotation)
        {
            _dbSet.Remove(entityShiftRotation);
            return true;
        }

    }
}
