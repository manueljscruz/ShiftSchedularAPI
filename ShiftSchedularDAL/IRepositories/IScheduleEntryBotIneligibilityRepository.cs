using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IScheduleEntryBotIneligibilityRepository : IGenericRepository<ScheduleEntryBotIneligibility>
    {
        Task<bool> DoesBotScheduleIneligibilityExist(Guid scheduleEntryId, Guid botId);
        Task<List<ScheduleEntryBotIneligibility>> GetScheduleEntryBotIneligibilities(Guid entityId, DateTime startDate, DateTime endDate);
        Task<List<ScheduleEntryBotIneligibility>> GetScheduleEntryBotIneligibilitiesByBotIdentifiers(Guid entityId, DateTime startDate, DateTime endDate, List<Guid> botIdentifiers);
    }
}
