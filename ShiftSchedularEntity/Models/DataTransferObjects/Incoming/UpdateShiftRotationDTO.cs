namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class UpdateShiftRotationDTO
    {
        public Guid EntityId { get; set; }
        public int OrderNo { get; set; }
        public bool IsLeave { get; set; }
        public int NewOrderNo { get; set; }
    }
}
