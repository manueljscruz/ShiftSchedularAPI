using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ShiftSchedularEntity.Entities.Base;

namespace ShiftSchedularEntity.Entities
{
    public class ShiftBreak : BaseEntity
    {
        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid ShiftBreakId { get; set; }

        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid ShiftId { get; set; }

        [Required]
        public int ShiftBreakTypeId { get; set; }
        public TimeSpan ShiftBreakStartTime { get; set; }
        public TimeSpan ShiftBreakDuration { get; set; }
        public bool IncludedInShift { get; set; }
        public bool IsTimeFlexible { get; set; }

        #region Navigation Properties

        public virtual Shift Shift { get; set; }
        public virtual ShiftBreakType ShiftBreakType { get; set; }

        #endregion
    }
}
