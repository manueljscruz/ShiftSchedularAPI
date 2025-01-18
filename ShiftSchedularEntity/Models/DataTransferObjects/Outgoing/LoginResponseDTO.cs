namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class LoginResponseDTO
    {
        public WorkerDTO User { get; set; }
        public TokenResponseDTO TokenResponseDTO { get; set; }
    }
}
