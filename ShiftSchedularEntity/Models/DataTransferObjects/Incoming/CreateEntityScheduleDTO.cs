namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class CreateEntityScheduleDTO : BaseViewModelRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool SingleRoleResponsibility { get; set; }
        public bool ClearExistingSchedule { get; set; }
        public bool ForceNoSkill { get; set; }
        public List<string> FilteredMembers { get; set; }
        public List<string> FilteredShifts { get; set; }
        public List<string> FilteredRules { get; set; }

        public CreateEntityScheduleDTO()
        {
            FilteredMembers = new List<string>();
            FilteredShifts = new List<string>();
            FilteredRules = new List<string>();
        }
    }
}
