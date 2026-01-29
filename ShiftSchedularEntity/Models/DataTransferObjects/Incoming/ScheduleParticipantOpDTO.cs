namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class ScheduleParticipantOpDTO
    {
        public string ScheduleEntryId { get; set; }
        public string WorkerId { get; set; }
        public bool IsEditing { get; set; }
        public bool IsRemoving { get; set; }

        public ScheduleParticipantOpDTO()
        {
            IsEditing = false;
            IsRemoving = false;
        }
    }
}
