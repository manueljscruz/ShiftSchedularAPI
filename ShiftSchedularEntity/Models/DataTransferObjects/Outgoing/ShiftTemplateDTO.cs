namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class ShiftTemplateDTO
    {
        public int ShiftTemplateId { get; set; }
        public string ShiftTemplateName { get; set; }
        public TimeSpan ShiftStartHour { get; set; }
        public TimeSpan ShiftDuration { get; set; }
        public bool IncludedInShift { get; set; }
        public bool IsTimeFlexible { get; set; }
        public bool IsPopular { get; set; }

        public List<ShiftBreakTemplateDTO> ShiftBreakTemplates { get; set; }
    }
}
