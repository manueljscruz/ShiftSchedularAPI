namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class DeleteEntityDTO
    {
        public Guid EntityId { get; set; }
        public string WorkerId { get; set; }
    }
}
