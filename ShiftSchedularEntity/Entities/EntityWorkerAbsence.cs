using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class EntityWorkerAbsence
    {
        /// <summary>
        /// Primary Key
        /// Identifier of the absence entry
        /// </summary>
        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid EntityWorkerAbsenceId { get; set; }

        /// <summary>
        /// Foreign Key - 1
        /// Identifier of the user
        /// </summary>
        [Required]
        public string ApplicationUserId { get; set; }

        /// <summary>
        /// Foreign Key - 2
        /// Identifier of the entity
        /// </summary>
        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid EntityId { get; set; }

        /// <summary>
        /// Foreign Key - 3
        /// Identifier of the absence type
        /// </summary>
        [Required]
        public int AbsenceTypeId { get; set; }

        /// <summary>
        /// Observations added to the absence submission
        /// </summary>
        [MaxLength(500)]
        public string Observations { get; set; }

        /// <summary>
        /// Start date of the absence
        /// </summary>
        public DateTime AbsenceStartDate { get; set; }

        /// <summary>
        /// End date of the absence
        /// </summary>
        public DateTime AbsenceEndDate { get; set; }

        /// <summary>
        /// Offset of the submission entry in minutes
        /// </summary>
        public int DateOffset { get; set; }

        /// <summary>
        /// Timezone id
        /// </summary>
        public string TimezoneId { get; set; }

        /// <summary>
        /// Flag that indicates if the absence has been approved
        /// </summary>
        public bool AbsenceApproved { get; set; }

        /// <summary>
        /// Identifier of the user that decided about the absence
        /// </summary>
        public string AbsenceDecisionOwner { get; set; }

        /// <summary>
        /// Date of the decision regarding the absence
        /// </summary>
        public DateTime AbsenceDateDecision { get; set; }

        /// <summary>
        /// Offset of the date when the decision was made in minutes
        /// </summary>
        public int AbsenceDateDecisionOffset { get; set; }

        public string DecisionTimezoneId { get; set; }

        #region Navigation Properties

        public virtual ApplicationUser? ApplicationUser { get; set; }
        public virtual Entity? Entity { get; set; }
        public virtual AbsenceType? AbsenceType { get; set; }

        #endregion
    }
}
