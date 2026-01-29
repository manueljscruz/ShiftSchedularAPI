namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class BaseViewModelRequest
    {
        public Guid EntityId { get; set; }
        public string WorkerId { get; set; }

        public BaseViewModelRequest()
        {
            EntityId = Guid.Empty;
            WorkerId = string.Empty;
        }
    }
}
