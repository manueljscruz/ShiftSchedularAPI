namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class CreateEntityScheduleDTO : BaseViewModelRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool SingleRoleResponsibility { get; set; }
        public bool ClearExistingSchedule { get; set; }
        public List<string> FilteredMembers { get; set; }
        public List<string> FilteredShifts { get; set; }
        public List<string> FilteredRules { get; set; }
    }
}
