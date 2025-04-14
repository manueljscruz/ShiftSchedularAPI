using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class EditMemberDTO
    {
        public string WorkerId { get; set; }
        public Guid EntityId { get; set; }
        public bool IsBot { get; set; }
        public string WorkerName { get; set; }
        public List<SkillLocalizedDTO> AssignedSkills { get; set; }
        public bool PartOfRotation { get; set; }
        public bool WorksWeekDays { get; set; }
        public bool WorksWeekends { get; set; }
        public List<ShiftDTO> AssignedShifts { get; set; }
    }
}
