using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.Queries;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.QueryModels;
using System.Data;

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

        #region Get By Entity Id

        public async Task<IEnumerable<EntityWorker>> GetByEntityId(Guid entityId)
        {
            if (!string.IsNullOrEmpty(entityId.ToString()))
            {
                return await _entityWorkerDbSet.Where(i => i.EntityId.Equals(entityId)).ToListAsync();
            }
            else
                return null;
        }

        #endregion

        #region Get By Worker and Entity

        public async Task<EntityWorker> GetByWorkerAndEntity(string workerId, Guid entityId)
        {
            if (!string.IsNullOrEmpty(workerId) && entityId != Guid.Empty)
            {
                try
                {
                    return await _entityWorkerDbSet
                        // Entity Worker Skills
                        .Include(ew => ew.ApplicationUser)
                            .ThenInclude(au => au.EntityWorkerSkills)

                        // Schedule Entry Workers
                        .Include(ew => ew.ApplicationUser)
                            .ThenInclude(au => au.ScheduleEntryWorkers)

                        // Entity Worker Shift Assigneds
                        .Include(ew => ew.ApplicationUser)
                            .ThenInclude(au => au.EntityWorkerShiftAssigneds)

                        // Schedule Entry Worker Ineligibilites
                        .Include(ew => ew.ApplicationUser)
                            .ThenInclude(au => au.ScheduleEntryWorkerIneligibilities)

                        .Where(i => i.EntityId.Equals(entityId) && i.ApplicationUserId.Equals(workerId)).FirstOrDefaultAsync();
                }
                catch (Exception ex)
                {
                    string strError = ex.Message;
                    return null;
                }

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
        public async Task<IEnumerable<EntityWorkerMemberModel>> GetDistinctMembersByEntityId(Guid entityId)
        {
            if (!string.IsNullOrEmpty(entityId.ToString()))
            {
                byte[] entityIdBytes = entityId.ToByteArray();

                Dictionary<string, object> parameters = new Dictionary<string, object>();

                parameters.Add("@EntityId", entityIdBytes);

                string query = string.Format(EntityWorkerSQL.GetDistinctEntityWorkersByEntityId, string.Empty);

                var result = await _sqlRawRepository.ExecuteQuery<EntityWorkerMemberModel>(query, parameters);

                return result;
            }
            else
                return null;
        }

        #endregion

        #region Get Distinct Members By Entity Id

        public async Task<IEnumerable<EntityWorkerMemberModel>> GetDistinctMembersByEntityId(Guid entityId, List<string> workers)
        {
            if (!string.IsNullOrEmpty(entityId.ToString()))
            {
                // Use LINQ with Contains to prevent SQL injection
                var query = from ew in _context.EntityWorkers
                           join w in _context.ApplicationUsers on ew.ApplicationUserId equals w.Id
                           where ew.EntityId == entityId && workers.Contains(w.Id)
                           select new EntityWorkerMemberModel
                           {
                               WorkerId = ew.ApplicationUserId,
                               WorkerName = w.DisplayName,
                               IsBot = false,
                               DateOfJoin = ew.DateOfJoin,
                               PartOfRotation = ew.PartOfRotation,
                               WorksWeekDays = ew.WorksWeekDays,
                               WorksWeekends = ew.WorksWeekends,
                               MultipleShiftAssignments = ew.MultipleShiftAssignments,
                               SkillIds = string.Join(",", _context.EntityWorkerSkills
                                   .Where(ews => ews.ApplicationUserId == ew.ApplicationUserId)
                                   .Select(ews => ews.SkillId))
                           };

                var result = await query.ToListAsync();
                return result;
            }
            else
                return null;
        }

        #endregion

        #region Get Distinct Skills By Entity Id

        public async Task<IEnumerable<int>> GetDistinctSkillsByEntityId(Guid entityId)
        {
            List<int> skillIds = new List<int>();

            if (!string.IsNullOrEmpty(entityId.ToString()))
            {
                byte[] entityIdBytes = entityId.ToByteArray();

                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("@EntityId", entityIdBytes);

                var result = await _sqlRawRepository.ExecuteQuery<int>(EntityWorkerSQL.GetDistinctEntitySkillsByEntityId, parameters);

                return result;
            }

            return skillIds;
        }

        #endregion

        #region Get Entity Owner Id

        public async Task<string> GetEntityOwnerId(Guid entityId)
        {
            if (!string.IsNullOrEmpty(entityId.ToString()))
            {
                EntityWorker entityWorker = await _entityWorkerDbSet.FirstOrDefaultAsync(i =>  i.EntityId.Equals(entityId)); // i.IsOwner &&

                if (entityWorker != null)
                    return entityWorker.ApplicationUserId;
            }
            return string.Empty;
        }

        #endregion

        #region Get Total Count By Entity 

        public async Task<int> GetTotalCountByEntity(Guid entityId)
        {
            int count = 0;
            if (!string.IsNullOrEmpty(entityId.ToString()))
            {
                byte[] entityIdBytes = entityId.ToByteArray();

                // GetEntityWorkersCount
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("@EntityId", entityIdBytes);

                var result = await _sqlRawRepository.ExecuteScalar<object>(EntityWorkerSQL.GetEntityWorkersCount, parameters);
                if (result != null)
                    count = Convert.ToInt32(result);
            }

            return count;
        }

        #endregion

        #region Is Worker In Entity

        public async Task<bool> IsWorkerInEntity(Guid entityId, string workerId)
        {
            if (string.IsNullOrEmpty(workerId))
                return false;

            byte[] entityIdBytes = entityId.ToByteArray();
            return await _entityWorkerDbSet
                .FromSqlRaw(
                    "SELECT * FROM [dbo].[EntityWorkers] WHERE EntityId = @entityId AND ApplicationUserId = @workerId AND IsDeleted = 0",
                    new SqlParameter("@entityId", SqlDbType.Binary) { Value = entityIdBytes, Size = 16 },
                    new SqlParameter("@workerId", workerId))
                .AnyAsync();
        }

        #endregion

        #region Get Simple By Worker And Entity

        /// <summary>
        /// Fetches the EntityWorker record without any navigation-property includes.
        /// Use for operations that only need the row itself (e.g. delete), to avoid
        /// exception-swallowing caused by missing or unmapped navigation properties
        /// in GetByWorkerAndEntity.
        /// </summary>
        public async Task<EntityWorker?> GetSimpleByWorkerAndEntity(string workerId, Guid entityId)
        {
            byte[] entityIdBytes = entityId.ToByteArray();
            return await _entityWorkerDbSet
                .FromSqlRaw(
                    "SELECT * FROM [dbo].[EntityWorkers] WHERE EntityId = @entityId AND ApplicationUserId = @workerId",
                    new SqlParameter("@entityId", System.Data.SqlDbType.Binary) { Value = entityIdBytes, Size = 16 },
                    new SqlParameter("@workerId", workerId))
                .FirstOrDefaultAsync();
        }

        #endregion

        #region Find By Worker And Entity (includes soft-deleted)

        public async Task<EntityWorker?> FindByWorkerAndEntity(string workerId, Guid entityId)
        {
            byte[] entityIdBytes = entityId.ToByteArray();
            return await _entityWorkerDbSet
                .FromSqlRaw(
                    "SELECT * FROM [dbo].[EntityWorkers] WHERE EntityId = @entityId AND ApplicationUserId = @workerId",
                    new SqlParameter("@entityId", SqlDbType.Binary) { Value = entityIdBytes, Size = 16 },
                    new SqlParameter("@workerId", workerId))
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync();
        }

        #endregion

        #region Delete By Entity And Worker

        public async Task DeleteByEntityAndWorker(Guid entityId, string workerId)
        {
            byte[] entityIdBytes = entityId.ToByteArray();
            EntityWorker entityWorker = await _entityWorkerDbSet
                .FromSqlRaw(
                    "SELECT * FROM [dbo].[EntityWorkers] WHERE EntityId = @entityId AND ApplicationUserId = @workerId",
                    new SqlParameter("@entityId", System.Data.SqlDbType.Binary) { Value = entityIdBytes, Size = 16 },
                    new SqlParameter("@workerId", workerId))
                .FirstOrDefaultAsync();
            if (entityWorker != null)
            {
                _entityWorkerDbSet.Remove(entityWorker);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        #endregion

        #region Is Member Owner

        public async Task<bool> IsMemberOwner(Guid entityId, string workerId)
        {
            bool result = false;

            if (!string.IsNullOrEmpty(workerId))
            {
                EntityWorker entityWorker = await _entityWorkerDbSet.Where(i => i.ApplicationUserId.Equals(workerId) && i.EntityId.Equals(entityId)).FirstOrDefaultAsync();
                if (entityWorker != null) // && entityWorker.IsOwner
                    result = true;
            }

            return result;
        }

        #endregion


    }
}
