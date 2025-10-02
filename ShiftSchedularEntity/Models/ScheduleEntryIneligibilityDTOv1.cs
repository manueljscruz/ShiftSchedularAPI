using ShiftSchedularEntity.Models.DataTransferObjects;

namespace ShiftSchedularEntity.Models
{
    public class ScheduleEntryIneligibilityDTOv1
    {
        public Guid ScheduleEntryID { get; set; }
        public List<EntityWorkerMemberDTO> MembersIneligible { get; set; }
        public DateTime ScheduleDate { get; set; }

        public ScheduleEntryIneligibilityDTOv1()
        {
            MembersIneligible = new List<EntityWorkerMemberDTO>();
        }

        public ScheduleEntryIneligibilityDTOv1(Guid scheduleEntryID, DateTime date) : base()
        {
            ScheduleEntryID = scheduleEntryID;
            MembersIneligible = new List<EntityWorkerMemberDTO>();
        }
    }
}
