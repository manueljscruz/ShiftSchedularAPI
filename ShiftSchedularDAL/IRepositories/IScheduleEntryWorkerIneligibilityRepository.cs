using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IScheduleEntryWorkerIneligibilityRepository : IGenericRepository<ScheduleEntryWorkerIneligibility>
    {
        Task<bool> DoesWorkerScheduleIneligibilityExist(Guid scheduleEntryId, string workerId);
        Task<List<ScheduleEntryWorkerIneligibility>> GetScheduleEntryWorkerIneligibilities(Guid entityId, DateTime startDate, DateTime endDate);
        Task<List<ScheduleEntryWorkerIneligibility>> GetScheduleEntryWorkerIneligibilitiesByWorkerIdentifiers(Guid entityId, DateTime startDate, DateTime endDate, List<string> workerIdentifiers);
    }
}
