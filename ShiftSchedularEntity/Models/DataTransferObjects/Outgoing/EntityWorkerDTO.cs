namespace ShiftSchedularEntity.Models.DataTransferObjects
{
    public class EntityWorkerDTO
    {
        public Guid EntityId { get; set; }
        public string EntityName { get; set; }
        public Guid? ParentEntityId { get; set; }

        /// <summary>
        /// The explicit permission role the worker holds at this entity.
        /// Null when the entity is an ancestor included for breadcrumb context only.
        /// </summary>
        public int? EntityPermissionRoleId { get; set; }
    }
}
