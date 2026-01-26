namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class SearchRequestDTO : PagedModelRequest
    {
        public string Query { get; set; }
        public string? ResultType { get; set; }
    }
}
