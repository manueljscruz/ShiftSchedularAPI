namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class EntityShiftViewModelRequestDTO
    {
        public Guid EntityId { get; set; }
        public string WorkerId { get; set; }

        public EntityShiftViewModelRequestDTO()
        {
            WorkerId = string.Empty;
        }
    }
}
