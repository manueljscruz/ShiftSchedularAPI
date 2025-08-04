namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class ApplyRotationCycleDTO : BaseViewModelRequest
    {
        public bool IsBot { get; set; }
        public DateTime CycleStartDate { get; set; }
        public DateTime CycleEndDate { get; set; }
    }
}
