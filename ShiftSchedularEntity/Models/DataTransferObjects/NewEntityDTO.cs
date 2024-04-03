namespace ShiftSchedularEntity.Models.DataTransferObjects
{
    public class NewEntityDTO
    {
        public string EntityName { get; set; }
        public int EntityTypeId { get; set; }
        public string EntityDescription { get; set; }
        public string WorkerId { get; set; }
    }
}
