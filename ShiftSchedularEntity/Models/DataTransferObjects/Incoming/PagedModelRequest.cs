namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class PagedModelRequest : BaseViewModelRequest
    {
        public int CurrentPage { get; set; }
        public int NextPage { get; set; }
        public int ItemsPerPage { get; set; }
        public bool ShowInactive { get; set; }

        public PagedModelRequest()
        {
            CurrentPage = 0;
            NextPage = 1;
            ItemsPerPage = 0;
            ShowInactive = false;
        }

        public PagedModelRequest(Guid entityId, string workerId, int currentPage, int nextPage, int itemsPerPage, bool showInactive)
        {
            EntityId = entityId;
            WorkerId = workerId;
            CurrentPage = currentPage;
            NextPage = nextPage;
            ItemsPerPage = itemsPerPage;
            ShowInactive = showInactive;
        }
    }
}
