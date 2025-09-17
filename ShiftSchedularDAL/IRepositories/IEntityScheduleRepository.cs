using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityScheduleRepository : IGenericRepository<ScheduleEntry>
    {
        Task<ScheduleEntry> GetById(Guid id);
        Task<List<ScheduleEntry>> GetScheduleEntries(Guid entityId, DateTime startDateSearch, DateTime endDateSearch);
        Task<List<ScheduleEntry>> GetScheduleEntries(Guid entityId, string workerId, DateTime startDateSearch, DateTime endDateSearch);
        Task<ScheduleEntry> GetByShiftAndDateEntry(Guid shiftId, DateTime date);
        Task<List<ScheduleEntry>> GetEFScheduleEntries(Guid entityId, DateTime startDateSearch, DateTime endDateSearch);
        Task<List<ScheduleEntry>> GetWorkerScheduleEntries(Guid entityId, DateTime startDateSearch, DateTime endDateSearch, Guid workerId, bool isBot);

    }
}
