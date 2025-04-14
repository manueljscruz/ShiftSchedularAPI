using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class EntityWorkerSkillRepository : GenericRepository<EntityWorkerSkill>, IEntityWorkerSkillRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<EntityWorkerSkill> _entityWorkerSkillsDbSet;
        private readonly IUnitOfWork _unitOfWork;
        public EntityWorkerSkillRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _entityWorkerSkillsDbSet = context.Set<EntityWorkerSkill>();
            _unitOfWork = unitOfWork;
        }


        public async Task<IEnumerable<EntityWorkerSkill>> GetByEntityId(Guid entityId)
        {
            if(entityId != Guid.Empty)
            {
                return _entityWorkerSkillsDbSet
                    .Where(x => x.EntityId == entityId);
            }
            else
            {
                return Enumerable.Empty<EntityWorkerSkill>();
            }
        }


        public Task<bool> DeleteAllByEntityIdAndUserId(Guid entityId, Guid userId)
        {
            if(entityId != Guid.Empty && userId != Guid.Empty)
            {
                IEnumerable<EntityWorkerSkill> entityWorkerSkills = _entityWorkerSkillsDbSet
                    .Where(x => x.EntityId.Equals(entityId) && x.ApplicationUserId.Equals(userId));
                _entityWorkerSkillsDbSet.RemoveRange(entityWorkerSkills);
                _unitOfWork.SaveChangesAsync();
                return Task.FromResult(true);
            }
            else return Task.FromResult(false);
        }
    }
}
