using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.Queries;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.QueryModels;

namespace ShiftSchedularDAL.Repositories
{
    public class EntityUserBotRepository : GenericRepository<EntityUserBot>, IEntityUserBotRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<EntityUserBot> _entityUserBotDbSet;
        private readonly ISQLRawRepository<object> _sqlRawRepository;

        public EntityUserBotRepository(DataContext context, IUnitOfWork unitOfWork, ISQLRawRepository<object> sqlRawRepository) : base(context, unitOfWork)
        {
            _context = context;
            _entityUserBotDbSet = _context.Set<EntityUserBot>();
            _sqlRawRepository = sqlRawRepository;
        }

        public async Task<bool> DeleteEntityUserBot(Guid entityId, Guid userBotId)
        {
            bool result = false;

            if (entityId != Guid.Empty && userBotId != Guid.Empty)
            {
                var entityUserBot = _entityUserBotDbSet.Where(x => x.EntityId.Equals(entityId) && x.UserBotId.Equals(userBotId)).FirstOrDefault();
                if (entityUserBot != null)
                {
                    _entityUserBotDbSet.Remove(entityUserBot);
                    _context.SaveChanges();
                    result = true;
                }
            }

            return result;
        }

        public async Task<IEnumerable<int>> GetDistinctSkillsByEntityId(Guid entityId)
        {
            List<int> skillIds = new List<int>();

            if (!string.IsNullOrEmpty(entityId.ToString()))
            {
                byte[] entityIdBytes = entityId.ToByteArray();

                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("@EntityId", entityIdBytes);

                var result = await _sqlRawRepository.ExecuteQuery<int>(EntityWorkerSQL.GetDistinctUserBotsSkillsByEntityId, parameters);

                return result;
            }

            return skillIds;
        }

        public async Task<IEnumerable<EntityWorkerMemberModel>> GetDistinctUserBotsByEntityId(Guid entityId)
        {
            if (!string.IsNullOrEmpty(entityId.ToString()))
            {
                byte[] entityIdBytes = entityId.ToByteArray();

                Dictionary<string, object> parameters = new Dictionary<string, object>();

                parameters.Add("@EntityId", entityId);

                string query = string.Format(EntityWorkerSQL.GetDistinctUserBotsByEntityId, string.Empty);

                var result = await _sqlRawRepository.ExecuteQuery<EntityWorkerMemberModel>(query, parameters);

                return result;
            }

            return null;
        }

        public async Task<EntityUserBot> GetEntityUserBotByEntityAndId(Guid entityId, Guid userId)
        {
            if (entityId != Guid.Empty && userId != Guid.Empty)
            {
                return _entityUserBotDbSet
                    // Entity User Bot Skills
                    .Include(eub => eub.UserBot)
                        .ThenInclude(ub => ub.EntityUserBotSkills)

                    // Entity User Bot Shift Assigneds
                    .Include(eub => eub.UserBot)
                        .ThenInclude(ub => ub.EntityUserBotShiftAssigneds)

                    // Schedule Entry Bots
                    .Include(eub => eub.UserBot)
                        .ThenInclude(ub => ub.ScheduleEntryBots)

                    // Schedule Entry Bot Ineligibilities
                    .Include(eub => eub.UserBot)
                        .ThenInclude(ub => ub.ScheduleEntryBotIneligibilities)

                    .Where(x => x.EntityId.Equals(entityId) && x.UserBotId.Equals(userId)).FirstOrDefault();
            }
            else return null;
        }

        public async Task<int> GetUserBotsByEntityCount(Guid entityId)
        {
            int count = 0;

            if (entityId != Guid.Empty)
            {

                byte[] entityIdBytes = entityId.ToByteArray();

                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("@EntityId", entityIdBytes);

                var result = await _sqlRawRepository.ExecuteScalar<object>(EntityWorkerSQL.GetEntityUserBotsCount, parameters);

                if (result != null)
                    count = Convert.ToInt32(result);
            }

            return count;
        }

        public async Task<IEnumerable<EntityUserBot>> GetUserBotsByEntityId(Guid entityId)
        {
            if (entityId != Guid.Empty)
            {
                return _entityUserBotDbSet.Where(x => x.EntityId.Equals(entityId));
            }
            else return null;
        }
    }
}
