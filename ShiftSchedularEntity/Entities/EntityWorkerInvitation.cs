namespace ShiftSchedularEntity.Entities
{
    public class EntityWorkerInvitation
    {
        // Primary Key and Foreign Key
        public string EntityId { get; set; }
        // Primary Key
        public string Email { get; set; }
        public string? WorkerId { get; set; }
        public DateTime InviteDate { get; set; }
        public string SkillsetIds { get; set; }

        #region Navigation Properties

        public virtual Entity Entity { get; set; }

        public virtual Worker? Worker { get; set; }

        #endregion
    }
}
