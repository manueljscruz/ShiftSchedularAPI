using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class EntityUserBotSkillRepository : GenericRepository<EntityUserBotSkill>, IEntityUserBotSkillRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<EntityUserBotSkill> _entityUserBotSkillsDbSet;
        private readonly IUnitOfWork _unitOfWork;

        public EntityUserBotSkillRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _entityUserBotSkillsDbSet = context.Set<EntityUserBotSkill>();
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<EntityUserBotSkill>> GetByEntityId(Guid entityId)
        {
            if (entityId != Guid.Empty)
            {
                return _entityUserBotSkillsDbSet
                    .Where(x => x.EntityId == entityId);
            }
            else
            {
                return Enumerable.Empty<EntityUserBotSkill>();
            }
        }

        public async Task<IEnumerable<EntityUserBotSkill>> GetByEntityIdAndUserBotId(Guid entityId, Guid userBotId)
        {
            if(entityId != Guid.Empty && userBotId != Guid.Empty)
            {
                return _entityUserBotSkillsDbSet
                    .Where(x => x.EntityId.Equals(entityId) && x.UserBotId.Equals(userBotId));
            }
            else
            {
                return null;
            }
        }


        public async Task<bool> DeleteAllByEntityIdAndUserBotId(Guid entityId, Guid userBotId)
        {
            if (entityId != Guid.Empty && userBotId != Guid.Empty)
            {
                IEnumerable<EntityUserBotSkill> entityUserBotSkills = await this.GetByEntityIdAndUserBotId(entityId, userBotId);
                _entityUserBotSkillsDbSet.RemoveRange(entityUserBotSkills);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            else return false;
        }

    }
}
