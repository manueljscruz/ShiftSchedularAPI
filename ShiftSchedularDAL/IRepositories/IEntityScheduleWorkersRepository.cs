using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityScheduleWorkersRepository : IGenericRepository<ScheduleEntryWorkers>
    {
        Task<IEnumerable<ScheduleEntryWorkers>> GetScheduleEntryWorkers(Guid scheduleEntryId);
    }
}
