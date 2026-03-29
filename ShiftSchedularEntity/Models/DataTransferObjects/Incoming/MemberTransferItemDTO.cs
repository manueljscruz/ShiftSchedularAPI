using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class MemberTransferItemDTO
    {
        public string WorkerId { get; set; }         // ApplicationUserId for humans; string form of UserBotId for bots
        public string WorkerName { get; set; }       // Display name (needed when copying a bot to create the new UserBot record)
        public bool IsBot { get; set; }
        // Human-only permission fields (ignored for bots)
        public int EntityPermissionRoleId { get; set; }
        public bool CanManageChildren { get; set; }
        public bool PartOfRoster { get; set; }
        // Scheduling fields
        public bool PartOfRotation { get; set; }
        public bool WorksWeekDays { get; set; }
        public bool WorksWeekends { get; set; }
        public bool MultipleShiftAssignments { get; set; }
        public List<SkillLocalizedDTO> AssignedSkills { get; set; } = new List<SkillLocalizedDTO>();
    }
}
