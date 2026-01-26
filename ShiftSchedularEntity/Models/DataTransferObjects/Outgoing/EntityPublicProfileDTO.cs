namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class EntityPublicProfileDTO
    {
        public Guid EntityId { get; set; }
        public string EntityName { get; set; }
        public string EntityTypeLocalized { get; set; }
        public string Description { get; set; }
    }
}
