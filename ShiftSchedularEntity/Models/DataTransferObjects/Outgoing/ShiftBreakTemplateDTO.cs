namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class ShiftBreakTemplateDTO
    {
        public int ShiftBreakTemplateId { get; set; }
        public int ShiftId { get; set; }
        public int ShiftBreakTypeId { get; set; }
        public string ShiftBreakTemplateName { get; set; }
        public string ShiftBreakTypeDisplayValue { get; set; }
        public TimeSpan ShiftBreakDuration { get; set; }
        public TimeSpan ShiftBreakStartHour { get; set; }
        public bool IncludedInShift { get; set; }
        public bool IsTimeFlexible { get; set; }
        public bool IsPopular { get; set; }
    }
}
