namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class HolidayCatalogLocalizedDTO
    {
        public int HolidayCatalogId { get; set; }
        public HolidayTypeLocalizedDTO HolidayTypeLocalized { get; set; }
        public HolidayBehaviourLocalizedDTO HolidayBehaviourLocalized { get; set; }
        public string HolidayCatalogLocalizedName { get; set; }
        public string HolidayCatalogLocalizedDescription { get; set; }
        public bool IsRecurring { get; set; }
        public int RecurrenceDay { get; set; }
        public int RecurrenceMonth { get; set; }
        public int IsActive { get; set; }
    }
}
