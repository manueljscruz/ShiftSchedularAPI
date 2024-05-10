using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class ShiftBreakType
    {
        #region Properties

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShiftBreakTypeId { get; set; }
        public string ShiftBreakTypeValue { get; set; }

        #endregion

        #region Navigation Properties

        public virtual IEnumerable<ShiftBreakTypeLocalization> ShiftBreakTypeLocalizations { get; set; }
        public virtual IEnumerable<ShiftBreak> ShiftBreaks { get; set; }
        public virtual IEnumerable<ShiftBreakTemplate> ShiftBreakTemplates { get; set; }

        #endregion
    }
}
