namespace ShiftSchedularEntity.Models.APIManagement
{
    public class ShiftTemplateSubmissionModel
    {
        public string ShiftName { get; set; }
        public string ShiftAlias { get; set; }
        public TimeSpan ShiftStartHour { get; set; }
        public TimeSpan ShiftDuration { get; set; }
        public List<int> ShiftBreakTemplateIdAssociations { get; set; }

        public ShiftTemplateSubmissionModel()
        {
            
        }

        public ShiftTemplateSubmissionModel(string shiftName, string shiftAlias, TimeSpan startHour, TimeSpan duration, List<int> shiftBreakTemplateIdAssociations)
        {
            ShiftName = shiftName;
            ShiftAlias = shiftAlias;
            ShiftStartHour = startHour;
            ShiftDuration = duration;
            ShiftBreakTemplateIdAssociations = shiftBreakTemplateIdAssociations;

        }
    }
}
