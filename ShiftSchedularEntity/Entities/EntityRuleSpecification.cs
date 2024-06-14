namespace ShiftSchedularEntity.Entities
{
    public class EntityRuleSpecification
    {
        public string EntityRuleId { get; set; }
        public int SpecificationId { get; set; }
        public int SpecificationValue { get; set; }
        public string AspectReferenceId { get; set; }
        public int BusinessAspectId { get; set; }
        public string AspectReferenceId2 { get; set; }
        public int BusinessAspectId2 { get; set; }

        #region Navigation Properties

        public virtual EntityRule EntityRule { get; set; }

        #endregion
    }
}
