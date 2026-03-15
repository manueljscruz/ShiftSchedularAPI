using ShiftSchedularEntity.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class EntityWorker : BaseEntity
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

        /// <summary>
        /// Determines if the worker is available for work on Weekdays
        public bool WorksWeekDays { get; set; }

        /// <summary>
        /// Determines if the worker is available for work on Weekends
        /// </summary>
        public bool WorksWeekends { get; set; }

        /// <summary>
        /// Determines if the worker is available to work more than one shift in a day
        /// </summary>
        public bool MultipleShiftAssignments { get; set; }

        public Guid? ConvertedFromBotId { get; set; }
        public DateTime? ConvertedAt { get; set; }
        public string? ConvertedBy { get; set; }

        #endregion

        #region Navigation Properties

        public virtual Entity Entity { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public UserBot? ConvertedFromBot { get; set; }
        public ApplicationUser? ConvertedByUser { get; set; }

        #endregion

    }
}
