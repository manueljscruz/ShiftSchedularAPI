namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class ImportCandidatesDTO
    {
        public List<ShiftSimpleDTO> Shifts { get; set; } = new();
        public List<EntityRuleSimpleDTO> Rules { get; set; } = new();
        public List<EntityHolidaySimpleDTO> Holidays { get; set; } = new();
    }

    public class ShiftSimpleDTO
    {
        public Guid ShiftId { get; set; }
        public string ShiftName { get; set; }
    }

    public class EntityRuleSimpleDTO
    {
        public Guid EntityRuleId { get; set; }
        public int RuleTypeId { get; set; }
        public string RuleTypeName { get; set; }
    }

    public class EntityHolidaySimpleDTO
    {
        public Guid EntityHolidayId { get; set; }
        public string HolidayDisplayName { get; set; }
        public int? HolidayCatalogId { get; set; }
        public int CustomDay { get; set; }
        public int CustomMonth { get; set; }
    }
}
