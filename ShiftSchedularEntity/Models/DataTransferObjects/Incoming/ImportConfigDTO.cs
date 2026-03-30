namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class ImportConfigDTO
    {
        public Guid DestinationEntityId { get; set; }
        public List<Guid> ShiftIds { get; set; } = new();
        public List<Guid> RuleIds { get; set; } = new();
        public List<Guid> HolidayIds { get; set; } = new();
    }
}
