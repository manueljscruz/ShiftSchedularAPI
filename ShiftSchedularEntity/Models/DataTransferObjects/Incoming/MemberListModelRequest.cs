namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class MemberListModelRequest : BaseViewModelRequest
    {
        public int CurrentPage { get; set; }
        public int NextPage { get; set; }
        public int ItemsPerPage { get; set; }

        public MemberListModelRequest()
        {
            CurrentPage = 0;
            NextPage = 1;
            ItemsPerPage = 0;
        }
    }
}
