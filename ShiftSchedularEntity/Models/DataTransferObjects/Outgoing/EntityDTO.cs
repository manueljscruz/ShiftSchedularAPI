namespace ShiftSchedularEntity.Models.DataTransferObjects
{
    public class EntityDTO
    {
        public string EntityId { get; set; }
        public string EntityName { get; set; }
        public string EntityDescription { get; set; }
        public string EntityTypeLocalized { get; set; }
        public int EntityWorkersCount { get; set; }

        public EntityDTO(string entityId, string entityName, string entityDescription, string entityTypeLocalized, int totalCount)
        {
            EntityId = entityId;
            EntityName = entityName;
            EntityDescription = entityDescription;
            EntityTypeLocalized = entityTypeLocalized;
            EntityWorkersCount = totalCount;
        }
    }
}
