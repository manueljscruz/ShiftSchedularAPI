using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.Queries;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
    }
}
