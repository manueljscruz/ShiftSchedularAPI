namespace ShiftSchedularEntity.Models.DataTransferObjects.Admin
{
    public class AdminHolidayCatalogItemDTO
    {
        public int Id { get; set; }
        public string HolidayName { get; set; }
        public string HolidayDescription { get; set; }
        public int HolidayTypeId { get; set; }
        public string HolidayTypeName { get; set; }
        public int HolidayBehaviourId { get; set; }
        public string HolidayBehaviourName { get; set; }
        public int RecurrenceDay { get; set; }
        public int RecurrenceMonth { get; set; }
        public bool IsRecurring { get; set; }
        public bool IsActive { get; set; }
        public List<AdminHolidayCatalogLocalizationValueDTO> Localizations { get; set; } = new();
    }

    public class AdminHolidayCatalogLocalizationValueDTO
    {
        public int LocalizationId { get; set; }
        public string LocalizationCode { get; set; }
        public string LocalizedName { get; set; }
        public string LocalizedDescription { get; set; }
    }

    public class AdminUpsertHolidayCatalogDTO
    {
        public int Id { get; set; }
        public string HolidayName { get; set; }
        public string HolidayDescription { get; set; }
        public int HolidayTypeId { get; set; }
        public int HolidayBehaviourId { get; set; }
        public int RecurrenceDay { get; set; }
        public int RecurrenceMonth { get; set; }
        public bool IsRecurring { get; set; }
        public bool IsActive { get; set; }
        public List<AdminUpsertHolidayCatalogLocalizationDTO> Localizations { get; set; } = new();
    }

    public class AdminUpsertHolidayCatalogLocalizationDTO
    {
        public int LocalizationId { get; set; }
        public string LocalizedName { get; set; }
        public string LocalizedDescription { get; set; }
    }
}
