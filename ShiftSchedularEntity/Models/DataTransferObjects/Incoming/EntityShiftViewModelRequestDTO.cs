namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class EntityShiftViewModelRequestDTO
    {
        public string EntityId { get; set; }
        public string WorkerId { get; set; }
        public string LanguageCode { get; set; }

        public EntityShiftViewModelRequestDTO()
        {
            EntityId = string.Empty;
            WorkerId = string.Empty;
            LanguageCode = string.Empty;
        }
    }
}
