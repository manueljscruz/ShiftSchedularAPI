using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class ScheduleEntryBotIneligibilityRepository : GenericRepository<ScheduleEntryBotIneligibility>, IScheduleEntryBotIneligibilityRepository
    {
        private readonly DataContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DbSet<ScheduleEntryBotIneligibility> _dbSet;

        #region Constructor

        public ScheduleEntryBotIneligibilityRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _dbSet = _context.Set<ScheduleEntryBotIneligibility>();
        }

        public async Task<bool> DoesBotScheduleIneligibilityExist(Guid scheduleEntryId, Guid botId)
        {
            return await _dbSet.AsNoTracking().AnyAsync(i => i.ScheduleEntryId.Equals(scheduleEntryId) && i.UserBotId.Equals(botId));
        }

        public async Task<List<ScheduleEntryBotIneligibility>> GetScheduleEntryBotIneligibilities(Guid entityId, DateTime startDate, DateTime endDate)
        {
            if (entityId != Guid.Empty)
            {
                var query =
                    _dbSet.Where(i => i.ScheduleEntry.Shift.EntityId.Equals(entityId) &&
                    i.ScheduleEntry.ScheduleStartDate <= endDate &&
                    i.ScheduleEntry.ScheduleEndDate >= startDate
                    );

                return query.ToList();
            }

            return new List<ScheduleEntryBotIneligibility>();
        }

        public async Task<List<ScheduleEntryBotIneligibility>> GetScheduleEntryBotIneligibilitiesByBotIdentifiers(Guid entityId, DateTime startDate, DateTime endDate, List<Guid> botIdentifiers)
        {
            if(entityId != Guid.Empty)
            {
                List<ScheduleEntryBotIneligibility> scheduleEntryBotIneligibilities = await GetScheduleEntryBotIneligibilities(entityId, startDate, endDate);
                return scheduleEntryBotIneligibilities.Where(i => botIdentifiers.Contains(i.UserBotId)).ToList();
            }
            return new List<ScheduleEntryBotIneligibility>();
        }

        #endregion


    }
}
