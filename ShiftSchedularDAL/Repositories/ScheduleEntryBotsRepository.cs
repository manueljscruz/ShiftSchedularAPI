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
    }
}
