namespace ShiftSchedularEntity.Models.DataTransferObjects.Admin
{
    public class AdminPaymentMethodTypeItemDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public List<string> CountryCodes { get; set; } = new();
        public List<AdminPaymentMethodTypeLocalizationValueDTO> Localizations { get; set; } = new();
    }

    public class AdminPaymentMethodTypeLocalizationValueDTO
    {
        public int LocalizationId { get; set; }
        public string LocalizationCode { get; set; }
        public string DisplayValue { get; set; }
    }

    public class AdminUpsertPaymentMethodTypeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public List<string> CountryCodes { get; set; } = new();
        public List<AdminUpsertPaymentMethodTypeLocalizationDTO> Localizations { get; set; } = new();
    }

    public class AdminUpsertPaymentMethodTypeLocalizationDTO
    {
        public int LocalizationId { get; set; }
        public string DisplayValue { get; set; }
    }
}
