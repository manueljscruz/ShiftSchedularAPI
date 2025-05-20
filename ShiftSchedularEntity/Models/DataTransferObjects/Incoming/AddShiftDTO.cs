namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class AddShiftDTO
    {
        public Guid EntityId { get; set; }
        public string ShiftName { get; set; }
        public string ShiftAlias { get; set; }
        public string ShiftDescription { get; set; }
        public TimeSpan ShiftStartHour { get; set; }
        public TimeSpan ShiftDuration { get; set; }
        public string ShiftColorHex { get; set; }
        public List<AddShiftBreakDTO> ShiftBreakDTOs { get; set; }
    }
}
