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
        Task<int> GetShiftForwardEntriesCount(Guid entityId, Guid shiftId, DateTime now);
        Task<bool> DeletePreviousShiftEntries(Guid entityId, Guid shiftId, DateTime now);

        /// <summary>
        /// Retrieves all schedule entries for a specific entity, including all child records.
        /// Intended for bulk operations such as deletion.
        /// </summary>
        /// <param name="entityId">The unique identifier of the entity</param>
        /// <returns>A collection of ScheduleEntry records with children eagerly loaded</returns>
        Task<List<ScheduleEntry>> GetByEntityId(Guid entityId);
    }
}
