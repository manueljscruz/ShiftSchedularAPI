namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class MemberExitDTO
    {
        public string WorkerId { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public bool IsBot { get; set; }
        public DateTime DateToExit { get; set; }
        public string ActingUserId { get; set; } = string.Empty;
    }
}
