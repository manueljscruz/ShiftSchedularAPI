using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.DbConstants
{
    public class BaseEntityRulesConstants
    {
        public static List<BaseEntityRule> BaseEntityRules = new List<BaseEntityRule>
        {
            new BaseEntityRule
            {
                BaseEntityRuleId = 1,
                RuleTypeId = 1,
            },
            new BaseEntityRule
            {
                BaseEntityRuleId = 2,
                RuleTypeId = 2,
            },
            new BaseEntityRule
            {
                BaseEntityRuleId = 3,
                RuleTypeId = 3,
            },
            new BaseEntityRule
            {
                BaseEntityRuleId = 4,
                RuleTypeId = 7,
            },
            new BaseEntityRule
            {
                BaseEntityRuleId = 5,
                RuleTypeId = 8,
            },
        };

        public static List<BaseEntityRuleSpecification> BaseEntityRuleSpecifications = new List<BaseEntityRuleSpecification>
        {
            new BaseEntityRuleSpecification
            {
                BaseEntityRuleId = 1,
                SpecificationId = 1,
                SpecificationValue = 16,
                BusinessAspectId = 0
            },
            new BaseEntityRuleSpecification
            {
                BaseEntityRuleId = 2,
                SpecificationId = 1,
                SpecificationValue = 48,
                BusinessAspectId = 0
            },
            new BaseEntityRuleSpecification
            {
                BaseEntityRuleId = 3,
                SpecificationId = 1,
                SpecificationValue = 1,
                BusinessAspectId = 0
            },
            new BaseEntityRuleSpecification
            {
                BaseEntityRuleId = 4,
                SpecificationId = 1,
                SpecificationValue = 1,
                BusinessAspectId = 0
            },
            new BaseEntityRuleSpecification
            {
                BaseEntityRuleId = 5,
                SpecificationId = 1,
                SpecificationValue = 0,
                BusinessAspectId = 0
            },
        };
    }
}
