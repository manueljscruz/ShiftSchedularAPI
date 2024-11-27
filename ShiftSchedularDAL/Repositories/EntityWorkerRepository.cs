using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.Queries;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.QueryModels;

namespace ShiftSchedularDAL.Repositories
{
    public class EntityWorkerRepository : GenericRepository<EntityWorker>, IEntityWorkerRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<EntityWorker> _entityWorkerDbSet;
        private readonly ISQLRawRepository<object> _sqlRawRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EntityWorkerRepository(DataContext context, IUnitOfWork unitOfWork, ISQLRawRepository<object> sqlRawRepository) : base(context, unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _sqlRawRepository = sqlRawRepository;
            _entityWorkerDbSet = _context.Set<EntityWorker>();
        }

        #region Get By Worker Id

        /// <summary>
        /// Gets entities to which the worker belongs to
        /// </summary>
        /// <param name="workerId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<EntityWorkerDTO>> GetByWorkerId(string workerId)
        {
            if (!string.IsNullOrEmpty(workerId))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("@WorkerId", workerId);

                return await _sqlRawRepository.ExecuteQuery<EntityWorkerDTO>(EntityWorkerSQL.GetEntityWorkersByWorkerId, parameters);
            }
            else
                return null;
        }

        #endregion

        #region Get By Entity Id

        public async Task<IEnumerable<EntityWorker>> GetByEntityId(string entityId)
        {
            if (!string.IsNullOrEmpty(entityId))
            {
                return await _entityWorkerDbSet.Where(i => i.EntityId.Equals(entityId)).ToListAsync();
            }
            else
                return null;
        }

        #endregion

        #region Get By Worker and Entity

        public async Task<List<EntityWorker>> GetByWorkerAndEntity(string workerId, string entityId)
        {
            if (!string.IsNullOrEmpty(workerId) && !string.IsNullOrEmpty(entityId))
            {
                return (List<EntityWorker>)_entityWorkerDbSet.Where(i => i.EntityId.Equals(entityId) && i.ApplicationUserId.Equals(workerId));
            }
            else return null;
        }

        #endregion

        #region Get Distinct Members By Entity Id

        /// <summary>
        /// Returns the members that belong to a specific entity and their skills with no duplicates
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<EntityWorkerMemberModel>> GetDistinctMembersByEntityId(string entityId)
        {
            if (!string.IsNullOrEmpty(entityId))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("@EntityId", entityId);

                string query = string.Format(EntityWorkerSQL.GetDistinctEntityWorkersByEntityId, string.Empty);

                return await _sqlRawRepository.ExecuteQuery<EntityWorkerMemberModel>(query, parameters);
            }
            else
                return null;
        }

        #endregion

        #region Get Distinct Members By Entity Id

        public async Task<IEnumerable<EntityWorkerMemberModel>> GetDistinctMembersByEntityId(string entityId, List<string> workers)
        {
            if (!string.IsNullOrEmpty(entityId))
            {
                string listInString = string.Join(",", workers.Select(v => $"'{v}'"));
                string filterFormat = string.Format(EntityWorkerSQL.GetDistinctEntityWorkersListFilter, listInString);

                string query = string.Format(EntityWorkerSQL.GetDistinctEntityWorkersByEntityId, filterFormat);
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("@EntityId", entityId);

                return await _sqlRawRepository.ExecuteQuery<EntityWorkerMemberModel>(query, parameters);


            }
            else
                return null;
        }

        #endregion

        #region Get Distinct Skills By Entity Id

        public async Task<IEnumerable<int>> GetDistinctSkillsByEntityId(string entityId)
        {
            List<int> skillIds = new List<int>();

            if (!string.IsNullOrEmpty(entityId))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("@EntityId", entityId);

                return await _sqlRawRepository.ExecuteQuery<int>(EntityWorkerSQL.GetDistinctEntitySkillsByEntityId, parameters);
            }

            return skillIds;
        }

        #endregion

        #region Get Entity Owner Id

        public async Task<string> GetEntityOwnerId(string entityId)
        {
            if (!string.IsNullOrEmpty(entityId))
            {
                EntityWorker entityWorker = await _entityWorkerDbSet.FirstOrDefaultAsync(i => i.IsOwner && i.EntityId.ToString() == entityId);

                if (entityWorker != null)
                    return entityWorker.ApplicationUserId;
            }
            return string.Empty;
        }

        #endregion

        #region Get Total Count By Entity 

        public async Task<int> GetTotalCountByEntity(Guid entityId)
        {
            if (entityId != Guid.Empty)
            {
                // GetEntityWorkersCount
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("@EntityId", entityId);

                var result = await _sqlRawRepository.ExecuteScalar<object>(EntityWorkerSQL.GetEntityWorkersCount, parameters);
                if (result != null)
                    return Convert.ToInt32(result);
            }
            return 0;
        }

        #endregion

        #region Is Worker In Entity

        public async Task<bool> IsWorkerInEntity(string entityId, string workerId)
        {
            if (string.IsNullOrEmpty(entityId) || string.IsNullOrEmpty(workerId))
            {
                return false;
            }

            return await _entityWorkerDbSet.AnyAsync(i => i.EntityId.Equals(entityId) && i.ApplicationUserId.Equals(workerId));

        }

        #endregion

        #region Is Member Owner

        public async Task<bool> IsMemberOwner(string entityId, string workerId)
        {
            bool result = false;

            if (!string.IsNullOrEmpty(workerId))
            {
                EntityWorker entityWorker = await _entityWorkerDbSet.Where(i => i.ApplicationUserId.Equals(workerId) && i.EntityId.Equals(entityId)).FirstOrDefaultAsync();
                if (entityWorker != null && entityWorker.IsOwner)
                    result = true;
            }

            return result;
        }

        #endregion

        
    }
}
