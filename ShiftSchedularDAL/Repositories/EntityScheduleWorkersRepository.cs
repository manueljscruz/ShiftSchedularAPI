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

        public async Task<IEnumerable<ScheduleEntryWorkers>> GetScheduleEntryWorkers(string scheduleEntryId)
        {
            if (!string.IsNullOrEmpty(scheduleEntryId))
                return await _dbSet.Where(x => x.ScheduleEntryId.ToString() == scheduleEntryId).ToListAsync();
            else
                return null;
        }

        #endregion

        #endregion
    }
}
