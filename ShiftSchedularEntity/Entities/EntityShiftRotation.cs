using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class EntityShiftRotation
    {
        #region Properties

        /// <summary>
        /// Composite Key - Entity Identifier
        /// </summary>
        [Key]
        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid EntityId { get; set; }

        /// <summary>
        /// Composite Key - Order Number
        /// </summary>
        [Required]
        public int OrderNo { get; set; }

        /// <summary>
        /// Composite Key - Flag that indicates if its a no work period
        /// </summary>
        [Required]
        public bool IsLeave { get; set; }

        /// <summary>
        /// Shift Identifier
        /// </summary>
        [Column(TypeName = "BINARY(16)")]
        public Guid? ShiftId { get; set; }

        /// <summary>
        /// When flag is leave, indicates the duration of the leave
        /// </summary>
        [Column(TypeName = "TIME")]
        [Range(typeof(TimeSpan), "00:00:00", "24:00:00")]
        public TimeSpan LeaveDuration { get; set; }

        #endregion

        #region Navigation Properties

        public virtual Entity Entity { get; set; }

        public virtual Shift Shift { get; set; }

        #endregion
    }
}
