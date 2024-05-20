namespace ShiftSchedularEntity.Models.APIManagement
{
    public class ShiftBreakTemplateSubmissionModel
    {
        public string ShiftTemplateName { get; set; }
        public int ShiftBreakTypeId { get; set; }
        public TimeSpan ShiftBreakStartTime { get; set; }
        public TimeSpan ShiftBreakDuration { get; set; }
        public bool IncludedInShift { get; set; }
        public bool IsTimeFlexible { get; set; }

        public ShiftBreakTemplateSubmissionModel(string shiftTemplateName, int shiftBreakTypeId, TimeSpan breakStartTime, TimeSpan breakDuration, bool includedInShift, bool isTimeFlexible)
        {
            ShiftTemplateName = shiftTemplateName;
            ShiftBreakTypeId = shiftBreakTypeId;
            ShiftBreakStartTime = breakStartTime;
            ShiftBreakDuration = breakDuration;
            IncludedInShift = includedInShift;
            IsTimeFlexible = isTimeFlexible;
        }
    }
}
