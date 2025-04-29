using ShiftSchedularEntity.Models.DataTransferObjects;

namespace ShiftSchedularEntity.Models
{
    public class ScheduleEntryIneligibility
    {
        public Guid ScheduleEntryID { get; set; }
        public List<EntityWorkerMemberDTO> MembersIneligible { get; set; }

        public ScheduleEntryIneligibility()
        {
            MembersIneligible = new List<EntityWorkerMemberDTO>();
        }

        public ScheduleEntryIneligibility(Guid scheduleEntryID) : base()
        {
            ScheduleEntryID = scheduleEntryID;
        }
    }
}
