using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IScheduleEntryBotsRepository : IGenericRepository<ScheduleEntryBots>
    {
        Task<IEnumerable<ScheduleEntryBots>> GetScheduleEntryBots(Guid scheduleEntryId);
    }
}
