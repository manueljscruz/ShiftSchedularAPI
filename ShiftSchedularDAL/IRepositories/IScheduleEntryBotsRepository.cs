using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IScheduleEntryBotsRepository : IGenericRepository<ScheduleEntryBots>
    {
        Task<IEnumerable<ScheduleEntryBots>> GetScheduleEntryBots(Guid scheduleEntryId);
        Task<bool> ParticipantExists(Guid scheduleEntryId, Guid workerId);
        Task<bool> DeleteScheduleEntryBot(Guid scheduleEntryId, Guid botId);
    }
}
