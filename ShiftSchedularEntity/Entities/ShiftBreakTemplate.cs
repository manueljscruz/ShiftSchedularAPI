namespace ShiftSchedularEntity.Entities
{
    public class ShiftBreakTemplate
    {
        public int ShiftBreakTemplateId { get; set; }
        public string ShiftBreakTemplateName { get; set; }
        public int ShiftBreakTypeId { get; set; }
        public TimeSpan ShiftBreakStartTime { get; set; }
        public TimeSpan ShiftBreakDuration { get; set; }
        public bool IncludedInShift { get; set; }
        public bool IsTimeFlexible { get; set; }
        public int TemplateClicks { get; set; }

        #region Navigation Properties

        public virtual IEnumerable<ShiftTemplateBreaks> ShiftTemplateBreaks { get; set; }
        public virtual ShiftBreakType ShiftBreakType { get; set; }

        #endregion
    }
}
