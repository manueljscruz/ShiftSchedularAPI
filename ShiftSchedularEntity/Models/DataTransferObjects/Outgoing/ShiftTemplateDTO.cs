namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class ShiftTemplateDTO
    {
        public int ShiftTemplateId { get; set; }
        public string ShiftTemplateName { get; set; }
        public string ShiftTemplateAlias { get; set; }
        public TimeSpan ShiftStartHour { get; set; }
        public TimeSpan ShiftDuration { get; set; }
        public bool IsPopular { get; set; }

        public List<ShiftBreakTemplateDTO> ShiftBreakTemplates { get; set; }

        public ShiftTemplateDTO()
        {
            ShiftBreakTemplates = new List<ShiftBreakTemplateDTO>();
        }
    }
}
