namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class DeleteMemberDTO
    {
        public string WorkerId { get; set; }
        public Guid EntityId { get; set; }
        public bool IsBot { get; set; }
    }
}
