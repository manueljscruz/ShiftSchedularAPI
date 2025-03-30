using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class Entity
    {
        #region Properties

        /// <summary>
        /// Primary Key
        /// Identifier of the entity
        /// </summary>
        [Key]
        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid EntityId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string EntityName { get; set; }

        /// <summary>
        /// Brief description of the entity
        /// </summary>
        [MaxLength(500)]
        public string EntityDescription { get; set; }

        /// <summary>
        /// Identifier of the type of entity
        /// </summary>
        public int EntityTypeId { get; set; }

        #endregion

        #region Navigation Properties

        public virtual EntityType EntityType { get; set; }
        public virtual ICollection<EntityWorker> EntityWorkers { get; set; }
        public virtual ICollection<EntityWorkerInvitation> EntityWorkerInvitations { get; set; }
        public virtual ICollection<Shift> EntityShifts { get; set; }
        public virtual ICollection<EntityRule> EntityRules { get; set; }
        public virtual ICollection<EntityWorkerAbsence> EntityWorkerAbsences { get; set; }
        public virtual ICollection<EntityUserBot> EntityUserBots { get; set; }
        public virtual ICollection<EntityShiftRotation> EntityShiftRotations { get; set; }
        public virtual ICollection<EntityWorkerShiftAssigned> EntityWorkerShiftAssigneds { get; set; }
        public virtual ICollection<EntityUserBotShiftAssigned> EntityUserBotShiftAssigneds { get; set; }

        #endregion
    }
}
