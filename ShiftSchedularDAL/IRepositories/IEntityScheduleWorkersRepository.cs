using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityScheduleWorkersRepository : IGenericRepository<ScheduleEntryWorkers>
    {
        Task<bool> DeleteScheduleEntryWorker(Guid scheduleEntryId, string applicationUserId);
        Task<IEnumerable<ScheduleEntryWorkers>> GetScheduleEntryWorkers(Guid scheduleEntryId);
        Task<bool> ParticipantExist(Guid scheduleEntryId, string workerId);
        Task<int> DeleteFutureWorkerParticipations(Guid entityId, string workerId, DateTime cutoffDate);
    }
}
