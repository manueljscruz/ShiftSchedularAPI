namespace ShiftSchedularEntity.Entities
{
    public class EntityRuleSpecification
    {
        public string EntityRuleId { get; set; }
        public int SpecificationId { get; set; }
        public int SpecificationValue { get; set; }
        public int BusinessAspectId { get; set; }
        public string AspectReferenceId { get; set; }

        #region Navigation Properties

        public virtual EntityRule EntityRule { get; set; }
        public virtual BusinessAspect? BusinessAspect { get; set; }

        #endregion
    }
}
