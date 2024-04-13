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

                return await _sqlRawRepository.ExecuteQuery<EntityWorkerMemberModel>(EntityWorkerSQL.GetDistinctEntityWorkersByEntityId, parameters);
            }
            else
                return null;
        }
    }
}
