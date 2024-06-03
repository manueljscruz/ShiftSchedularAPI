namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class BaseViewModelRequest
    {
        public string EntityId { get; set; }
        public string WorkerId { get; set; }
        public string LanguageCode { get; set; }

        public BaseViewModelRequest()
        {
            EntityId = string.Empty;
            WorkerId = string.Empty;
            LanguageCode = string.Empty;
        }
    }
}
