namespace ShiftSchedularEntity.Entities
{
    public class BaseEntityRuleSpecification
    {
        #region Properties

        public int BaseEntityRuleId { get; set; }
        public int SpecificationId { get; set; }
        public int SpecificationValue { get; set; }
        public int BusinessAspectId { get; set; }

        #endregion

        #region Navigation Properties

        public virtual BaseEntityRule BaseEntityRule { get; set; }

        #endregion
    }
}
