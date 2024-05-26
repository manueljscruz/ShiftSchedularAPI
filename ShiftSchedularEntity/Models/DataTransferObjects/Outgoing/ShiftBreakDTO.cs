namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class ShiftBreakDTO
    {
        public string ShiftBreakId { get; set; }
        public string ShiftParentId { get; set; }
        public int ShiftBreakTypeId { get; set; }
        public string ShiftBreakTypeDisplay { get; set; }
        public TimeSpan ShiftBreakStartTime { get; set; }
        public TimeSpan ShiftBreakDuration { get; set; }
        public bool IncludedInShift { get; set; }
        public bool IsTimeFlexible { get; set; }
    }
}
