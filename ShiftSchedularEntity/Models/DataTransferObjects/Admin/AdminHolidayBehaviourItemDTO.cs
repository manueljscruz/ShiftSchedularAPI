namespace ShiftSchedularEntity.Models.DataTransferObjects.Admin
{
    public class AdminHolidayBehaviourItemDTO
    {
        public int Id { get; set; }
        public string InternalName { get; set; }
        public bool AllowsOperatingTimes { get; set; }
        public List<AdminLocalizationValueDTO> Localizations { get; set; } = new();
    }

    public class AdminUpsertHolidayBehaviourDTO
    {
        public int Id { get; set; }
        public string InternalName { get; set; }
        public bool AllowsOperatingTimes { get; set; }
        public List<AdminUpsertLocalizationDTO> Localizations { get; set; } = new();
    }
}
