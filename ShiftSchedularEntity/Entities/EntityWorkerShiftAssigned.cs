using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ShiftSchedularEntity.Entities.Base;

namespace ShiftSchedularEntity.Entities
{
    public class EntityWorkerShiftAssigned : BaseEntity
    {
        #region Properties

        [Required]
        public string ApplicationUserId { get; set; }

        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid EntityId { get; set; }

        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid ShiftId { get; set; }

        #endregion

        #region Navigation Properties

        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Entity Entity { get; set; }
        public virtual Shift Shift { get; set; }

        #endregion
    }
}
