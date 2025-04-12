using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class EntityWorker
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
        /// User Identifier
        /// </summary>
        [Required]
        public string ApplicationUserId { get; set; }

        /// <summary>
        /// Composite Key - 3
        /// Skill Identifier
        /// </summary>
        [Required]
        public int SkillId { get; set; }

        /// <summary>
        /// Part of the active worker roster
        /// </summary>
        public bool ActiveWorkerStatus { get; set; }

        /// <summary>
        /// Flag indicator of being a manager/owner of the entity
        /// </summary>
        public bool IsOwner { get; set; }

        /// <summary>
        /// Flag indicator of being able to create schedules
        /// </summary>
        public bool CanCreateSchedules { get; set; }

        /// <summary>
        /// Date of joining the entity workforce
        /// </summary>
        public DateTime DateOfJoin { get; set; }

        /// <summary>
        /// Date of exiting the entity workforce
        /// </summary>
        public DateTime DateToExit { get; set; }

        /// <summary>
        /// Signal if its part of the rotation
        /// </summary>
        public bool PartOfRotation { get; set; }

        //public bool WorksWeekDays { get; set; }

        //public bool WorksWeekends { get; set; }

        #endregion

        #region Navigation Properties

        public virtual Entity Entity { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Skill Skill { get; set; }

        #endregion

    }
}
