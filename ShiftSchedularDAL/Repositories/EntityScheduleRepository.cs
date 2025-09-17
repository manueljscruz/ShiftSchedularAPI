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
                var query = _scheduleEntriesDbSet.Where
                    (i => i.Shift.EntityId.Equals(entityId)
                    && i.ScheduleStartDate >= startDateSearch
                    && i.ScheduleEndDate <= endDateSearch)
                    .Include(i => i.ScheduleEntryWorkers)
                    .Include(i => i.ScheduleEntryBots);

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

        public async Task<ScheduleEntry> GetById(Guid id)
        {
            if (id != Guid.Empty)
            {
                return await _scheduleEntriesDbSet.Include(i => i.ScheduleEntryBots)
                    .Include(i => i.ScheduleEntryWorkers)
                    .Where(i => i.ScheduleEntryId.Equals(id)).FirstOrDefaultAsync();
            }
            else
                return null;
        }

        #endregion

    }
}
