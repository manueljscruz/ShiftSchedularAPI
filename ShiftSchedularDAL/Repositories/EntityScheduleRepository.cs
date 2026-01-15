using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.Queries;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class EntityScheduleRepository : GenericRepository<ScheduleEntry>, IEntityScheduleRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<ScheduleEntry> _scheduleEntriesDbSet;
        private readonly ILocalizationRepository _localizationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISQLRawRepository<object> _sqlRawRepository;

        public EntityScheduleRepository(DataContext context, IUnitOfWork unitOfWork, ISQLRawRepository<object> sqlRawRepository) : base(context, unitOfWork)
        {
            _context = context;
            _scheduleEntriesDbSet = _context.Set<ScheduleEntry>();
            _unitOfWork = unitOfWork;
            _sqlRawRepository = sqlRawRepository;
        }

        public async Task<List<ScheduleEntry>> GetScheduleEntries(Guid entityId, DateTime startDateSearch, DateTime endDateSearch)
        {
            if (entityId != Guid.Empty)
            {
                try
                {
                    IEnumerable<ScheduleEntry> scheduleEntries = _scheduleEntriesDbSet.Where(i => i.Shift.EntityId.Equals(entityId)
                        && i.ScheduleStartDate.Date >= startDateSearch.Date
                        && i.ScheduleEndDate.Date <= endDateSearch.Date).OrderBy(i => i.ScheduleStartDate);

                    return scheduleEntries.ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }

            }
            else
                return null;
        }

        public async Task<List<ScheduleEntry>> GetScheduleEntries(Guid entityId, string workerId, DateTime startDateSearch, DateTime endDateSearch)
        {
            if (entityId != Guid.Empty)
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("@EntityId", entityId);
                parameters.Add("@StartDateSearch", startDateSearch);
                parameters.Add("@EndDateSearch", endDateSearch);
                parameters.Add("@WorkerId", workerId);

                IEnumerable<ScheduleEntry> scheduleEntries = await _sqlRawRepository.ExecuteQuery<ScheduleEntry>(ScheduleEntrySQL.GetWorkerScheduleEntries, parameters);
                return scheduleEntries.ToList();
            }
            else
                return null;
        }

        public async Task<List<ScheduleEntry>> GetEFScheduleEntries(Guid entityId, DateTime startDateSearch, DateTime endDateSearch)
        {
            if (entityId != Guid.Empty)
            {
                var query = _scheduleEntriesDbSet
                .Where(i => i.Shift.EntityId.Equals(entityId)
                    && i.ScheduleStartDate <= endDateSearch
                    && i.ScheduleEndDate >= startDateSearch)
                .Include(i => i.Shift)  // Eager load Shift to prevent N+1
                .Include(i => i.ScheduleEntryWorkers)
                .Include(i => i.ScheduleEntryBots)
                .Include(i => i.ScheduleEntryWorkerIneligibilities)
                .Include(i => i.ScheduleEntryBotIneligibilities)
                .OrderBy(i => i.ScheduleStartDate);


                List<ScheduleEntry> scheduleEntries = await query.ToListAsync();
                return scheduleEntries;
            }
            else
                return null;
        }

        public async Task<List<ScheduleEntry>> GetWorkerScheduleEntries(Guid entityId, DateTime startDateSearch, DateTime endDateSearch, Guid workerId, bool isBot)
        {
            if (entityId != Guid.Empty)
            {
                List<ScheduleEntry> scheduleEntries = await this.GetEFScheduleEntries(entityId, startDateSearch, endDateSearch);


                if (isBot)
                {
                    scheduleEntries = scheduleEntries
                        .Where(i => i.ScheduleEntryBots.Any(j => j.UserBotId.Equals(workerId)))
                        .Select(i =>
                        {
                            i.ScheduleEntryBots = i.ScheduleEntryBots
                                .Where(j => j.UserBotId.Equals(workerId))
                                .ToList();
                            return i;
                        })
                        .ToList();
                }
                else
                {
                    scheduleEntries = scheduleEntries
                        .Where(i => i.ScheduleEntryWorkers.Any(j => j.ApplicationUserId.Equals(workerId.ToString())))
                        .Select(i =>
                        {
                            i.ScheduleEntryWorkers = i.ScheduleEntryWorkers
                                .Where(j => j.ApplicationUserId.Equals(workerId.ToString()))
                                .ToList();
                            return i;
                        })
                        .ToList();
                }


                return scheduleEntries;
            }
            else
                return null;
        }

        #region Get By Shift And Date Entry

        public async Task<ScheduleEntry> GetByShiftAndDateEntry(Guid shiftId, DateTime date)
        {
            if (shiftId != Guid.Empty)
            {
                return await _scheduleEntriesDbSet
                    .Include(i => i.ScheduleEntryBots)
                    .Include(i => i.ScheduleEntryWorkers)
                    .Where(i => i.ShiftId.Equals(shiftId) && i.ScheduleStartDate.Date.Equals(date.Date)).FirstOrDefaultAsync();
            }
            else
                return null;

        }

        #endregion

        #region Get By Id

        public async Task<ScheduleEntry> GetById(Guid id)
        {
            if (id != Guid.Empty)
            {
                return await _scheduleEntriesDbSet
                    .Include(i => i.Shift)  // Eager load Shift to prevent N+1
                    .Include(i => i.ScheduleEntryBots)
                    .Include(i => i.ScheduleEntryWorkers)
                    .Where(i => i.ScheduleEntryId.Equals(id)).FirstOrDefaultAsync();
            }
            else
                return null;
        }

        #endregion

        #region Get Shift Forward Entries Count

        public async Task<int> GetShiftForwardEntriesCount(Guid entityId, Guid shiftId, DateTime now)
        {
            int count = 0;

            if (entityId != Guid.Empty && shiftId != Guid.Empty)
            {
                count = await _scheduleEntriesDbSet.Where(i => i.ShiftId.Equals(shiftId)
                    && i.Shift.EntityId.Equals(entityId)
                    && i.ScheduleStartDate.Date >= now.Date).CountAsync();
            }
            else
                count = -1;


            return count;
        }

        #endregion

        #region Delete Previous Shift Entries

        public async Task<bool> DeletePreviousShiftEntries(Guid entityId, Guid shiftId, DateTime dateOfTermination)
        {
            if (entityId == Guid.Empty && shiftId == Guid.Empty)
                return false;

            IEnumerable<ScheduleEntry> scheduleEntries = await _scheduleEntriesDbSet
                .Include(i => i.ScheduleEntryWorkers)
                .Include(i => i.ScheduleEntryBots)
                .Include(i => i.ScheduleEntryBotIneligibilities)
                .Include(i => i.ScheduleEntryWorkerIneligibilities)
                .Where(i => i.ShiftId.Equals(shiftId)
                && i.Shift.EntityId.Equals(entityId)
                && i.ScheduleStartDate.Date <= dateOfTermination.Date)
                .ToListAsync();

            if (scheduleEntries.Count() == 0)
                return true; // ✅ Nothing to delete, still a successful outcome

            await _unitOfWork.ScheduleEntryBotsRepository.DeleteRange(scheduleEntries.SelectMany(i => i.ScheduleEntryBots));
            await _unitOfWork.ScheduleEntryBotIneligibilityRepository.DeleteRange(scheduleEntries.SelectMany(i => i.ScheduleEntryBotIneligibilities));
            await _unitOfWork.EntityScheduleWorkersRepository.DeleteRange(scheduleEntries.SelectMany(i => i.ScheduleEntryWorkers));
            await _unitOfWork.ScheduleEntryWorkerIneligibilityRepository.DeleteRange(scheduleEntries.SelectMany(i => i.ScheduleEntryWorkerIneligibilities));

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        #endregion
    }
}
