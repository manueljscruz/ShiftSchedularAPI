namespace ShiftSchedularEntity.Models.DataTransferObjects
{
    public class EntityDTO
    {
        public Guid EntityId { get; set; }
        public string EntityName { get; set; }
        public string EntityDescription { get; set; }
        public string EntityTypeLocalized { get; set; }
        public int EntityWorkersCount { get; set; }
        public Guid? ParentEntityId { get; set; }

        public EntityDTO(Guid entityId, string entityName, string entityDescription, string entityTypeLocalized, int totalCount, Guid? parentEntityId = null)
        {
            EntityId = entityId;
            EntityName = entityName;
            EntityDescription = entityDescription;
            EntityTypeLocalized = entityTypeLocalized;
            EntityWorkersCount = totalCount;
            ParentEntityId = parentEntityId;
        }
    }
}
