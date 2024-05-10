namespace ShiftSchedularEntity.Entities
{
    public class ShiftBreak
    {
        public string ShiftBreakId { get; set; }
        public string ShiftId { get; set; }
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
