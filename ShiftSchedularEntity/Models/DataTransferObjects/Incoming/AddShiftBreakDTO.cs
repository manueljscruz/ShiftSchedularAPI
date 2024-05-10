namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class AddShiftBreakDTO
    {
        public string ShiftId { get; set; }
        public int ShiftBreakTypeId { get; set; }
        public TimeSpan ShiftBreakStartTime { get; set; }
        public TimeSpan ShiftBreakDuration { get; set; }
        public bool IncludedInShift { get; set; }
        public bool IsTimeFlexible { get; set; }
    }
}
