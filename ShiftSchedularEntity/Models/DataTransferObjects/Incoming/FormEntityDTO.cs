namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class FormEntityDTO
    {
        public Guid EntityId { get; set; }
        public string EntityName { get; set; }
        public int EntityTypeId { get; set; }
        public string EntityDescription { get; set; }
        public string WorkerId { get; set; }
    }
}
