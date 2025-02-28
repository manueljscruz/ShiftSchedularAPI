namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class BaseViewModelRequest
    {
        public Guid EntityId { get; set; }
        public string WorkerId { get; set; }
        public string LanguageCode { get; set; }

        public BaseViewModelRequest()
        {
            EntityId = Guid.Empty;
            WorkerId = string.Empty;
            LanguageCode = string.Empty;
        }
    }
}
