namespace ShiftSchedularEntity.Entities
{
    public class ShiftTemplate
    {
        public int ShiftTemplateId { get; set; }
        public string ShiftName { get; set; }
        public string ShiftAlias { get; set; }
        public TimeSpan ShiftStartHour { get; set; }
        public TimeSpan ShiftDuration { get; set; }
        public int TemplateClicks { get; set; }

        #region Navigation Properties

        public virtual IEnumerable<ShiftTemplateBreaks> ShiftTemplateBreaks { get; set; }

        #endregion
    }
}
