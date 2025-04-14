using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class EntityWorkerSkill
    {
        #region Properties

        /// <summary>
        /// Composite key - 1
        /// User ID
        /// </summary>
        [Required]
        public string ApplicationUserId { get; set; }

        /// <summary>
        /// Composite Key - 2
        /// Entity Identifier
        /// </summary>
        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid EntityId { get; set; }

        /// <summary>
        /// Composite Key - 3
        /// Skill Identifier
        /// </summary>
        [Required]
        public int SkillId { get; set; }

        #endregion

        #region Navigation Properties

        public virtual Skill Skill { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Entity Entity { get; set; }

        #endregion
    }
}
