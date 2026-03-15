using ShiftSchedularEntity.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class EntityUserBotSkill : BaseEntity
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
        /// Composite key - 2
        /// User Bot Identifier
        /// </summary>
        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid UserBotId { get; set; }

        /// <summary>
        /// Composite key - 3
        /// Skill Identifier
        /// </summary>
        [Required]
        public int SkillId { get; set; }

        #endregion

        #region Navigation Properties

        public virtual Entity Entity { get; set; }
        public virtual UserBot UserBot { get; set; }
        public virtual Skill Skill { get; set; }

        #endregion
    }
}
