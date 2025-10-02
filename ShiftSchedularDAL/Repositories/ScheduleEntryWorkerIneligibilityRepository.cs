using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class ScheduleEntryWorkerIneligibilityRepository : GenericRepository<ScheduleEntryWorkerIneligibility>, IScheduleEntryWorkerIneligibilityRepository
    {
        private readonly DataContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DbSet<ScheduleEntryWorkerIneligibility> _dbSet;

        #region Constructor

        public ScheduleEntryWorkerIneligibilityRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _dbSet = _context.Set<ScheduleEntryWorkerIneligibility>();
        }

        public async Task<bool> DoesWorkerScheduleIneligibilityExist(Guid scheduleEntryId, string workerId)
        {
            return await _dbSet.AsNoTracking().AnyAsync(i => i.ScheduleEntryId.Equals(scheduleEntryId) && i.ApplicationUserId.Equals(workerId));
        }

        public async Task<List<ScheduleEntryWorkerIneligibility>> GetScheduleEntryWorkerIneligibilities(Guid entityId, DateTime startDate, DateTime endDate)
        {
            if(entityId != Guid.Empty)
            {
                var query =
                    _dbSet.Where(i => i.ScheduleEntry.Shift.EntityId.Equals(entityId) &&
                    i.ScheduleEntry.ScheduleStartDate <= endDate &&
                    i.ScheduleEntry.ScheduleEndDate >= startDate
                    );

                return query.ToList();
            }

            return new List<ScheduleEntryWorkerIneligibility>();
        }

        public async Task<List<ScheduleEntryWorkerIneligibility>> GetScheduleEntryWorkerIneligibilitiesByWorkerIdentifiers(Guid entityId, DateTime startDate, DateTime endDate, List<string> workerIdentifiers)
        {
            if (entityId != Guid.Empty)
            {
                List<ScheduleEntryWorkerIneligibility> scheduleEntryBotIneligibilities = await GetScheduleEntryWorkerIneligibilities(entityId, startDate, endDate);
                return scheduleEntryBotIneligibilities.Where(i => workerIdentifiers.Contains(i.ApplicationUserId)).ToList();
            }
            return new List<ScheduleEntryWorkerIneligibility>();
        }

        #endregion
    }
}
