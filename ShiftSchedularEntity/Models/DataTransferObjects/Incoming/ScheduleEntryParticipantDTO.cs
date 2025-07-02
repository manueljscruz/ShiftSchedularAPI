namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class ScheduleEntryParticipantDTO
    {
        public EntityWorkerMemberDTO Worker { get; set; }
        public List<SkillLocalizedDTO> AssignedSkills { get; set; }
    }
}
