using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularEntity.Models
{
    public class WorkerUnderhoursSchedule
    {
        public EntityWorkerMemberDTO Worker { get; set; }
        public IEnumerable<ScheduleEntryDTO> ToBeAssignedEntries { get; set; }
    }
}
