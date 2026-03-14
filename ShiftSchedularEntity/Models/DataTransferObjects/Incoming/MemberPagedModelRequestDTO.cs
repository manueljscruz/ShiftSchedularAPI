namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class MemberPagedModelRequestDTO : PagedModelRequest
    {
        public MemberListFilterDTO? MemberFilters { get; set; }
    }
}
