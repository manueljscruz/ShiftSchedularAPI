namespace ShiftSchedularEntity.Models.DataTransferObjects.Admin
{
    public class AdminRuleTypeItemDTO
    {
        public int Id { get; set; }
        public string InternalName { get; set; }
        public string Description { get; set; }
        public bool MultipleSpecification { get; set; }
        public bool IsSpecValuesBoolean { get; set; }
        public int OrderNo { get; set; }
        public List<int> BusinessAspectIds { get; set; } = new();
        public List<AdminRuleTypeLocalizationValueDTO> Localizations { get; set; } = new();
    }

    public class AdminRuleTypeLocalizationValueDTO
    {
        public int LocalizationId { get; set; }
        public string LocalizationCode { get; set; }
        public string DisplayValue { get; set; }
        public string DescriptionValue { get; set; }
    }

    public class AdminUpsertRuleTypeDTO
    {
        public int Id { get; set; }
        public string InternalName { get; set; }
        public string Description { get; set; }
        public bool MultipleSpecification { get; set; }
        public bool IsSpecValuesBoolean { get; set; }
        public int OrderNo { get; set; }
        public List<int> BusinessAspectIds { get; set; } = new();
        public List<AdminUpsertRuleTypeLocalizationDTO> Localizations { get; set; } = new();
    }

    public class AdminUpsertRuleTypeLocalizationDTO
    {
        public int LocalizationId { get; set; }
        public string DisplayValue { get; set; }
        public string DescriptionValue { get; set; }
    }
}
