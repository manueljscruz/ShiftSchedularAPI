namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class EntityShiftRotationDTO
    {
        public Guid EntityId { get; set; }
        public int OrderNo { get; set; }
        public bool IsLeave { get; set; }
        public Guid ShiftId { get; set; }
        public string DisplayName { get; set; }
        public string Alias { get; set; }
        public TimeSpan LeaveDuration { get; set; }
    }
}
