namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class ScheduleViewModelRequest : BaseViewModelRequest
    {
        public DateTime StartDateSearch { get; set; }
        public DateTime EndDateSearch { get; set; }
    }
}
