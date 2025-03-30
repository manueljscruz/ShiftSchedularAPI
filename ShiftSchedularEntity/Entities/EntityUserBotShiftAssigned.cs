using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class EntityUserBotShiftAssigned
    {
        #region Properties

        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid UserBotId { get; set; }

        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid EntityId { get; set; }

        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid ShiftId { get; set; }

        #endregion

        #region Navigation Properties

        public virtual UserBot UserBot { get; set; }
        public virtual Entity Entity { get; set; }
        public virtual Shift Shift { get; set; }

        #endregion
    }
}
