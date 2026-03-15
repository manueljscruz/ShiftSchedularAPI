namespace ShiftSchedularEntity.Entities.Base
{
    public class BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public string? CreatedById { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedById { get; set; }


        #region Navigation Properties

        public virtual ApplicationUser? CreatedByUser { get; set; }
        public virtual ApplicationUser? UpdatedByUser { get; set; }
        public virtual ApplicationUser? DeletedByUser { get; set; }

        #endregion
    }
}
