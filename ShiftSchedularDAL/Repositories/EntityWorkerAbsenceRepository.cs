using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class EntityWorkerAbsenceRepository : GenericRepository<EntityWorkerAbsence>, IEntityWorkerAbsenceRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<EntityWorkerAbsence> _dbSet;
        private readonly IUnitOfWork _unitOfWork;

        public EntityWorkerAbsenceRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _dbSet = context.Set<EntityWorkerAbsence>();
        }

        #region Get Entity Worker Absences

        /// <summary>
        /// Returns a list of all the Entity Worker Absences
        /// If its the owner requesting data, it will retrieve all of the data
        /// Otherwise, it will show only the data of the user
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="workerId"></param>
        /// <param name="isOwner"></param>
        /// <returns></returns>
        public async Task<IEnumerable<EntityWorkerAbsence>> GetEntityWorkerAbsences(Guid entityId, string workerId, bool isOwner)
        {
            if (isOwner)
                return await _dbSet.Where(i => i.EntityId.Equals(entityId)).ToListAsync();
            else
                return await _dbSet.Where(i => i.EntityId.Equals(entityId) && i.ApplicationUserId.Equals(workerId)).ToListAsync();
        }

        #endregion

        #region Get Specific Worker Absences

        public async Task<IEnumerable<EntityWorkerAbsence>> GetSpecificWorkerAbsences(Guid entityId, List<string> workers, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (entityId != Guid.Empty && workers.Count != 0)
            {
                var data = await _dbSet
                    .Where(i => i.EntityId.Equals(entityId) && workers.Contains(i.ApplicationUserId))
                    .ToListAsync();

                if (startDate != null && endDate != null)
                {
                    data = data.Where(i =>
                        i.AbsenceStartDate < endDate.Value &&
                        i.AbsenceEndDate > startDate.Value
                    ).ToList();
                }

                return data;
            }

            return Enumerable.Empty<EntityWorkerAbsence>();
        }

        #endregion
    }
}
