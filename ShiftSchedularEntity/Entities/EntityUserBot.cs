using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ShiftSchedularEntity.Entities.Base;

namespace ShiftSchedularEntity.Entities
{
    public class EntityUserBot : BaseEntity
    {
        #region Properties

        /// <summary>
        /// Composite key - 1
        /// Entity Identifier
        /// </summary>
        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid EntityId { get; set; }

        /// <summary>
        /// Composite Key - 2
        /// User Bot Identifier
        /// </summary>
        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid UserBotId { get; set; }

        public bool ActiveWorkerStatus { get; set; }

        public DateTime DateOfJoin { get; set; }

        public DateTime DateOfExit { get; set; }

        public bool PartOfRotation { get; set; }

        public bool WorksWeekDays { get; set; }

        public bool WorksWeekends { get; set; }

        public bool MultipleShiftAssignments { get; set; }

        #endregion

        #region Navigation Properties

        public virtual Entity Entity { get; set; }
        public virtual UserBot UserBot { get; set; }

        #endregion
    }
}
