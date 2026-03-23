namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class PendingInvitationDTO
    {
        public Guid EntityId { get; set; }
        public string EntityName { get; set; }
        public DateTime InviteDate { get; set; }
        public int EntityPermissionRoleId { get; set; }
    }
}
