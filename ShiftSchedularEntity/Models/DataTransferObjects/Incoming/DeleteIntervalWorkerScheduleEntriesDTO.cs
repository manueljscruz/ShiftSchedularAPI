namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class DeleteIntervalWorkerScheduleEntriesDTO : BaseViewModelRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
