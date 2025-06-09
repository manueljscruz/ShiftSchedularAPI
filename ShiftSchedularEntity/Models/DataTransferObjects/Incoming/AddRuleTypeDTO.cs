namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class AddRuleTypeDTO
    {
        #region Properties

        public string NewRuleType { get; set; }
        public bool MultipleSpecification { get; set; }
        public bool IsSpecValuesBoolean { get; set; }
        public string RuleTypeDescription { get; set; }
        public int OrderNo { get; set; }

        #endregion

        #region Constructor

        public AddRuleTypeDTO()
        {
            NewRuleType = string.Empty;
            MultipleSpecification = false;
            IsSpecValuesBoolean = false;
            RuleTypeDescription = string.Empty;
        }

        #endregion
    }
}
