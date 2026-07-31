using ShiftSchedularEntity.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class Entity : BaseEntity
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

        /// <summary>
        /// Identifier of a parent Entity if applicable
        /// </summary>
        public Guid? ParentEntityId { get; set; }

        #endregion

        #region Navigation Properties

        public virtual EntityType EntityType { get; set; }
        public virtual ICollection<EntityWorker> EntityWorkers { get; set; }
        public virtual ICollection<EntityWorkerSkill> EntityWorkerSkills { get; set; }
        public virtual ICollection<EntityWorkerInvitation> EntityWorkerInvitations { get; set; }
        public virtual ICollection<Shift> EntityShifts { get; set; }
        public virtual ICollection<EntityRule> EntityRules { get; set; }
        public virtual ICollection<EntityWorkerAbsence> EntityWorkerAbsences { get; set; }
        public virtual ICollection<EntityUserBot> EntityUserBots { get; set; }
        public virtual ICollection<EntityUserBotSkill> EntityUserBotSkills { get; set; }
        public virtual ICollection<EntityShiftRotation> EntityShiftRotations { get; set; }
        public virtual ICollection<EntityWorkerShiftAssigned> EntityWorkerShiftAssigneds { get; set; }
        public virtual ICollection<EntityUserBotShiftAssigned> EntityUserBotShiftAssigneds { get; set; }
        public virtual ICollection<EntityHoliday> EntityHolidays { get; set; }
        public virtual Entity? ParentEntity { get; set; }
        public virtual ICollection<Entity> ChildrenEntities { get; set; }
        public virtual ICollection<EntityPermission> EntityPermissions { get; set; }
        public virtual ICollection<EntitySubscriptionPlan> EntitySubscriptionPlans { get; set; }
        public virtual ICollection<PaymentMethod> PaymentMethods { get; set; }
        public virtual ICollection<EntityBillingProfile> EntityBillingProfiles { get; set; }

        #endregion
    }
}
