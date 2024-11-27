namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class ShiftDTO
    {
        public Guid ShiftId { get; set; }
        public Guid EntityId { get; set; }
        public string ShiftName { get; set; }
        public string ShiftAlias { get; set; }
        public string ShiftDescription { get; set; }
        public TimeSpan ShiftStartHour { get; set; }
        public TimeSpan ShiftDuration { get; set; }
        public List<ShiftBreakDTO> ShiftBreakDTOs { get; set; }

        public ShiftDTO()
        {
            ShiftBreakDTOs = new List<ShiftBreakDTO>();
        }
    }
}
