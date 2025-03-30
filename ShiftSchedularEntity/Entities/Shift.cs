using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class Shift
    {
        #region Properties

        /// <summary>
        /// Primary Key
        /// Shift Identifier
        /// </summary>
        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid ShiftId { get; set; }

        /// <summary>
        /// Foreign Key
        /// Identifier of the entity
        /// </summary>
        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid EntityId { get; set; }

        /// <summary>
        /// Name of the shift
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string ShiftName { get; set; }

        /// <summary>
        /// Alias given to a shift by the entity owner
        /// </summary>
        public string ShiftAlias { get; set; }

        /// <summary>
        /// Description of the shift
        /// </summary>
        [MaxLength(500)]
        public string ShiftDescription { get; set; }

        /// <summary>
        /// Start hour of the Shift
        /// </summary>
        [Column(TypeName = "TIME")]
        [Range(typeof(TimeSpan), "00:00:00", "24:00:00")]
        public TimeSpan ShiftStartHour { get; set; }

        /// <summary>
        /// Duration of the shift
        /// </summary>
        [Column(TypeName = "TIME")]
        [Range(typeof(TimeSpan), "00:00:00", "24:00:00")]
        public TimeSpan ShiftDuration { get; set; }

        #endregion

        #region Navigation Properties

        public virtual Entity Entity { get; set; }
        public virtual IEnumerable<ShiftBreak> ShiftBreaks { get; set; }
        public virtual IEnumerable<ScheduleEntry> ScheduleEntries { get; set; }
        public virtual IEnumerable<EntityUserBotShiftAssigned> EntityUserBotShiftAssigned { get; set; }
        public virtual IEnumerable<EntityWorkerShiftAssigned> EntityWorkerShiftAssigned { get; set; }

        #endregion
    }
}
