namespace ShiftSchedularEntity.Models.DataTransferObjects.Admin
{
    public class AdminUpsertTypeDTO
    {
        public int Id { get; set; }
        public string InternalName { get; set; }
        public string? HexBGColor { get; set; }
        public string? HexFontColor { get; set; }
        public List<AdminUpsertLocalizationDTO> Localizations { get; set; } = new();
    }
}
