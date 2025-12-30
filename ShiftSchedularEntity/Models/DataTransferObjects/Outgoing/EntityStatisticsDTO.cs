namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class EntityStatisticsDTO
    {

        public int NumberOfMembers { get; set; }
        public int NumberOfRules { get; set; }
        public int NumberOfShifts { get; set; }

        public EntityStatisticsDTO(int numberOfMembers, int numberOfRules, int numberOfShifts)
        {
            NumberOfMembers = numberOfMembers;
            NumberOfRules = numberOfRules;
            NumberOfShifts = numberOfShifts;
        }
    }
}
