namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class ShiftDTO
    {
        public string ShiftId { get; set; }
        public string EntityId { get; set; }
        public string ShiftName { get; set; }
        public string ShiftAlias { get; set; }
        public string ShiftDescription { get; set; }
        public TimeSpan ShiftStartHour { get; set; }
        public TimeSpan ShiftDuration { get; set; }
        public List<ShiftBreakDTO> ShiftBreakDTOs { get; set; }
    }
}
