namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class NewWorkerDTO
    {
        public string WorkerName { get; set; }
        public int GenderId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
