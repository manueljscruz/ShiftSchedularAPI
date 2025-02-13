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

        public async Task<IEnumerable<EntityWorkerMemberModel>> GetDistinctUserBotsByEntityId(Guid entityId)
        {
            if (!string.IsNullOrEmpty(entityId.ToString()))
            {
                byte[] entityIdBytes = entityId.ToByteArray();

                Dictionary<string, object> parameters = new Dictionary<string, object>();

                parameters.Add("@EntityId", entityId);

                string query = string.Format(EntityWorkerSQL.GetDistinctUserBotsByEntityId, string.Empty);

                return await _sqlRawRepository.ExecuteQuery<EntityWorkerMemberModel>(query, parameters);
            }
            else
                return null;
        }

        public async Task<IEnumerable<EntityUserBot>> GetEntityUserBotsByEntityAndId(Guid entityId, Guid userId)
        {
            if (entityId != Guid.Empty && userId != Guid.Empty)
            {
                return _entityUserBotDbSet.Where(x => x.EntityId.Equals(entityId) && x.UserBotId.Equals(userId));
            }
            else return null;
        }
    }
}
