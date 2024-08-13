using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityScheduleRepository : IGenericRepository<ScheduleEntry>
    {
        Task<List<ScheduleEntry>> GetScheduleEntries(string entityId, string workerId, DateTime startDateSearch, DateTime endDateSearch);
    }
}
