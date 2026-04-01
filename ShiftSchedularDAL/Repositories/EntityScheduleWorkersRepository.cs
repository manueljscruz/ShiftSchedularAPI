using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class EntityScheduleWorkersRepository : GenericRepository<ScheduleEntryWorkers>, IEntityScheduleWorkersRepository
    {
        #region Properties

        private readonly IUnitOfWork _unitOfWork;
        private readonly DataContext _context;
        private readonly DbSet<ScheduleEntryWorkers> _dbSet;

        #endregion

        #region Constructor

        public EntityScheduleWorkersRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _dbSet = _context.Set<ScheduleEntryWorkers>();
        }

        #endregion

        #region Methods

        #region Get Schedule Entry Workers

        public async Task<IEnumerable<ScheduleEntryWorkers>> GetScheduleEntryWorkers(Guid scheduleEntryId)
        {
            if (scheduleEntryId != Guid.Empty)
                return await _dbSet.Where(x => x.ScheduleEntryId.Equals(scheduleEntryId)).ToListAsync();
            else
                return null;
        }

        #endregion

        #region Participant Exist

        public async Task<bool> ParticipantExist(Guid scheduleEntryId, string workerId)
        {
            if(scheduleEntryId != Guid.Empty)
            {
                return _dbSet.Any(i => i.ScheduleEntryId.Equals(scheduleEntryId) && i.ApplicationUserId.Equals(workerId));
            }

            return false;
        }

        #endregion


        public async Task<bool> DeleteScheduleEntryWorker(Guid scheduleEntryId, string applicationUserId)
        {
            if(scheduleEntryId != Guid.Empty)
            {
                ScheduleEntryWorkers scheduleEntryWorkerInstance = await _dbSet.Where(i => i.ScheduleEntryId.Equals(scheduleEntryId) && i.ApplicationUserId.Equals(applicationUserId)).FirstOrDefaultAsync();

                if(scheduleEntryWorkerInstance != null)
                {
                    _dbSet.Remove(scheduleEntryWorkerInstance);
                    await _unitOfWork.SaveChangesAsync();
                    return true;
                }
            }

            return false;
        }

        public async Task<int> DeleteFutureWorkerParticipations(Guid entityId, string workerId, DateTime cutoffDate)
        {
            return await _dbSet
                .Where(sew =>
                    sew.ApplicationUserId == workerId &&
                    sew.ScheduleEntry.ScheduleStartDate.Date > cutoffDate.Date &&
                    sew.ScheduleEntry.Shift.EntityId == entityId)
                .ExecuteDeleteAsync();
        }


        #endregion
    }
}
