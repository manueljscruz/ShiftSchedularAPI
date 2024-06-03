namespace ShiftSchedularEntity.Models.APIManagement
{
    public class RuleTypeLocalizationSubmissionModel : BaseLocalizationSubmissionModel
    {
        public int RuleTypeId { get; set; }

        public RuleTypeLocalizationSubmissionModel() : base()
        {
            RuleTypeId = 0;
        }

        public RuleTypeLocalizationSubmissionModel(int ruleTypeId) : base() 
        {
            RuleTypeId = ruleTypeId;
        }
    }
}
