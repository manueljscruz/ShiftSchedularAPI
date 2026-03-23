namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class UpdateMemberPermissionDTO
    {
        public string WorkerId { get; set; }
        public Guid EntityId { get; set; }
        public int EntityPermissionRoleId { get; set; }
        public bool CanManageChildren { get; set; }
        public bool PartOfRoster { get; set; }
    }
}
