namespace ShiftSchedularEntity.Entities
{
    public class ShiftTemplateBreaks
    {
        public int ShiftTemplateId { get; set; }
        public int ShiftBreakTemplateId { get; set; }

        #region Navigation Properties

        public virtual ShiftTemplate ShiftTemplate { get; set; }
        public virtual ShiftBreakTemplate ShiftBreakTemplate { get; set; }

        #endregion
    }
}
