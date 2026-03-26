using ShiftSchedularEntity.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class EntityWorkerInvitation : BaseEntity
    {
        /// <summary>
        /// Composite Key - 1
        /// Identifier of the Entity
        /// </summary>
        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid EntityId { get; set; }

        /// <summary>
        /// Composite Key - 2
        /// Email of the user
        /// The user can be not registered at the moment
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        /// <summary>
        /// Identifier of the user
        /// </summary>
        public string? ApplicationUserId { get; set; }

        /// <summary>
        /// Date of the invitation
        /// </summary>
        public DateTime InviteDate { get; set; }

        /// <summary>
        /// Assigned skill identifiers set by entity owner
        /// </summary>
        public string SkillsetIds { get; set; }

        public bool PartOfRotation { get; set; }

        public bool WorksWeekDays { get; set; }

        public bool WorksWeekends { get; set; }

        public bool MultipleShiftAssignments { get; set; }

        public int EntityPermissionRoleId { get; set; }

        public bool PartOfRoster { get; set; }

        #region Navigation Properties

        public virtual Entity? Entity { get; set; }

        public virtual ApplicationUser? ApplicationUser { get; set; }

        #endregion
    }
}
