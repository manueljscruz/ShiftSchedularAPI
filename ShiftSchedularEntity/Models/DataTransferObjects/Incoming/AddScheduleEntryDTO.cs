namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class AddScheduleEntryDTO
    {
        public string ShiftId { get; set; }
        public DateTime ScheduleStartDate { get; set; }
        public string LanguageCode { get; set; }

        public AddScheduleEntryDTO() 
        { 
        }
    }
}
