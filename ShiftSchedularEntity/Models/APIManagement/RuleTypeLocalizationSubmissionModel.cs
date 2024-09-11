namespace ShiftSchedularEntity.Models.APIManagement
{
    public class RuleTypeLocalizationSubmissionModel : BaseLocalizationSubmissionModel
    {
        public int RuleTypeId { get; set; }
        public string RuleTypeDescription { get; set; }

        public RuleTypeLocalizationSubmissionModel() : base()
        {
            RuleTypeId = 0;
            RuleTypeDescription = string.Empty;
        }

        public RuleTypeLocalizationSubmissionModel(int ruleTypeId, string ruleTypeDescription) : base() 
        {
            RuleTypeId = ruleTypeId;
            RuleTypeDescription = ruleTypeDescription;
        }
    }
}
