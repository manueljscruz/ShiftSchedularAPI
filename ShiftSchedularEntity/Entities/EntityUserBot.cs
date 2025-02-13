using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    public class EntityUserBot
    {
        #region Properties

        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid EntityId { get; set; }

        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid UserBotId { get; set; }

        [Required]
        public int SkillId { get; set; }

        public bool ActiveWorkerStatus { get; set; }

        public DateTime DateOfJoin { get; set; }

        public DateTime DateOfExit { get; set; }

        #endregion

        #region Navigation Properties

        public virtual Entity Entity { get; set; }
        public virtual UserBot UserBot { get; set; }
        public virtual Skill Skill { get; set; }

        #endregion
    }
}
