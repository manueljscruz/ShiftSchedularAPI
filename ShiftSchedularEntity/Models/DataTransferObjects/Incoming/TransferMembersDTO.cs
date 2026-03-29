using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class TransferMembersDTO
    {
        public Guid SourceEntityId { get; set; }
        public Guid DestinationEntityId { get; set; }
        /// <summary>
        /// true = move (remove from source after adding to destination)
        /// false = copy (member remains in source)
        /// </summary>
        public bool IsTransfer { get; set; }
        public List<MemberTransferItemDTO> Members { get; set; } = new List<MemberTransferItemDTO>();
    }
}
