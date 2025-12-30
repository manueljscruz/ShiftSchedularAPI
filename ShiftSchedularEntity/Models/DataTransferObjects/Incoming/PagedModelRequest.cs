namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class PagedModelRequest : BaseViewModelRequest
    {
        public int CurrentPage { get; set; }
        public int NextPage { get; set; }
        public int ItemsPerPage { get; set; }

        public PagedModelRequest()
        {
            CurrentPage = 0;
            NextPage = 1;
            ItemsPerPage = 0;
        }

        public PagedModelRequest(Guid entityId, string workerId, string languageCode, int currentPage, int nextPage, int itemsPerPage)
        {
            EntityId = entityId;
            WorkerId = workerId;
            LanguageCode = languageCode;
            CurrentPage = currentPage;
            NextPage = nextPage;
            ItemsPerPage = itemsPerPage;
        }
    }
}
