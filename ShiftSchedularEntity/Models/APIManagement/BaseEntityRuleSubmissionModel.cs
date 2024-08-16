namespace ShiftSchedularEntity.Models.APIManagement
{
    public class BaseEntityRuleSubmissionModel
    {
        #region Properties

        public int RuleTypeId { get; set; }
        public List<BaseEntityRuleSpecificationSubmissionModel> BaseEntityRuleSpecificationSubmissionModels { get; set; }

        #endregion

        #region Constructor

        public BaseEntityRuleSubmissionModel()
        {
            BaseEntityRuleSpecificationSubmissionModels = new List<BaseEntityRuleSpecificationSubmissionModel>();
        }

        #endregion
    }
}
