namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class EditMemberDTO
    {
        public string WorkerId { get; set; }
        public string EntityId { get; set; }
        public bool IsBot { get; set; }
        public string WorkerName { get; set; }
        public List<SkillLocalizedDTO> AssignedSkills { get; set; }
    }
}
