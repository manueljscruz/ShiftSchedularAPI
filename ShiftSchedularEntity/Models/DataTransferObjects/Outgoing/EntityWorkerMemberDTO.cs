using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularEntity.Models.DataTransferObjects
{
    public class EntityWorkerMemberDTO
    {
        public string WorkerId { get; set; }
        public string WorkerName { get; set; }
        public bool IsBot { get; set; }
        public DateTime DateOfJoin { get; set; }
        public bool PartOfRotation { get; set; }
        public bool WorksWeekDays { get; set; }
        public bool WorksWeekends { get; set; }
        public bool MultipleShiftAssignments { get; set; }
        public List<SkillLocalizedDTO> SkillSet { get; set; }
        public List<ShiftDTO> AssignedShifts { get; set; }
        public EntityWorkerMemberDTO()
        {
            SkillSet = new List<SkillLocalizedDTO>();
            AssignedShifts = new List<ShiftDTO>();
        }
    }
}
