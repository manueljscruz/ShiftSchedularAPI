using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityScheduleRepository : IGenericRepository<ScheduleEntry>
    {
        Task<List<ScheduleEntry>> GetScheduleEntries(Guid entityId, string workerId, DateTime startDateSearch, DateTime endDateSearch);
    }
}
