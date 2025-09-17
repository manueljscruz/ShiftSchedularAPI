using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class ScheduleEntryBotsRepository : GenericRepository<ScheduleEntryBots>, IScheduleEntryBotsRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DataContext _context;
        private readonly DbSet<ScheduleEntryBots> _dbSet;

        public ScheduleEntryBotsRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _dbSet = _context.Set<ScheduleEntryBots>();
        }

        public async Task<IEnumerable<ScheduleEntryBots>> GetScheduleEntryBots(Guid scheduleEntryId)
        {
            if (scheduleEntryId != Guid.Empty)
                return await _dbSet.Where(x => x.ScheduleEntryId.Equals(scheduleEntryId)).ToListAsync();
            else
                return null;
        }

        public async Task<bool> ParticipantExists(Guid scheduleEntryId, Guid workerId)
        {
            if (scheduleEntryId != Guid.Empty)
            {
                return _dbSet.Any(i => i.ScheduleEntryId.Equals(scheduleEntryId) && i.UserBotId.Equals(workerId));
            }
            return false;

        }

        #region Delete Schedule Entry Bot

        public async Task<bool> DeleteScheduleEntryBot(Guid scheduleEntryId, Guid botId)
        {
            if (scheduleEntryId != Guid.Empty)
            {
                ScheduleEntryBots scheduleEntryBotInstance = await _dbSet.Where(i => i.ScheduleEntryId.Equals(scheduleEntryId) && i.UserBotId.Equals(botId)).FirstOrDefaultAsync();
                
                if(scheduleEntryBotInstance != null)
                {
                    _dbSet.Remove(scheduleEntryBotInstance);
                    await _unitOfWork.SaveChangesAsync();
                    return true;
                }
            }

            return false;
        }

        #endregion

    }
}
