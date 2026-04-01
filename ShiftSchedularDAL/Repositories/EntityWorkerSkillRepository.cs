using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using System.Data;

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


        public async Task<EntityWorkerSkill?> FindByWorkerEntityAndSkill(string workerId, Guid entityId, int skillId)
        {
            byte[] entityIdBytes = entityId.ToByteArray();
            return await _entityWorkerSkillsDbSet
                .FromSqlRaw(
                    "SELECT * FROM [dbo].[EntityWorkerSkills] WHERE ApplicationUserId = @workerId AND EntityId = @entityId AND SkillId = @skillId",
                    new SqlParameter("@workerId", workerId),
                    new SqlParameter("@entityId", SqlDbType.Binary) { Value = entityIdBytes, Size = 16 },
                    new SqlParameter("@skillId", skillId))
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteAllByEntityIdAndUserId(Guid entityId, Guid userId)
        {
            if(entityId != Guid.Empty && userId != Guid.Empty)
            {
                string userIdStr = userId.ToString();
                await _entityWorkerSkillsDbSet
                    .Where(x => x.EntityId == entityId && x.ApplicationUserId == userIdStr)
                    .ExecuteDeleteAsync();
                return true;
            }
            else return false;
        }
    }
}
