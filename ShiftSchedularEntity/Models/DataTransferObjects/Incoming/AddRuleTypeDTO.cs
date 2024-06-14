namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class AddRuleTypeDTO
    {
        #region Properties

        public string NewRuleType { get; set; }
        public bool MultipleSpecification { get; set; }

        #endregion

        #region Constructor

        public AddRuleTypeDTO()
        {
            NewRuleType = string.Empty;
            MultipleSpecification = false;
        }

        #endregion
    }
}
