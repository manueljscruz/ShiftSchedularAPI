namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class EntityRuleSpecificationDTO
    {
        /// <summary>
        /// Identifier of the entity rule
        /// </summary>
        public string EntityRuleId { get; set; }

        /// <summary>
        /// Number of the specification
        /// </summary>
        public int SpecificationId { get; set; }

        /// <summary>
        /// Specified value by the user regarding this rule
        /// </summary>
        public int RuleSpecificationValue { get; set; }

        /// <summary>
        /// First Reference identifier
        /// </summary>
        public string AspectReferenceId { get; set; }

        /// <summary>
        /// Business Aspect identifier
        /// </summary>
        public int BusinessAspectId { get; set; }

        /// <summary>
        /// Business Aspect localized Value for the first reference
        /// </summary>
        public string BusinessAspectDisplayValue { get; set; }

        /// <summary>
        /// Name of the Reference
        /// </summary>
        public string ReferenceName { get; set; }

        /// <summary>
        /// Second reference identifier
        /// </summary>
        public string AspectReferenceId2 { get; set; }

        /// <summary>
        /// Business Aspect of the second reference identifier
        /// </summary>
        public int BusinessAspectId2 { get; set; }

        /// <summary>
        /// Business Aspect localized value of the second reference
        /// </summary>
        public string BusinessAspect2DisplayValue { get; set; }

        /// <summary>
        /// Name of the second reference
        /// </summary>
        public string ReferenceName2 { get; set; }

        /// <summary>
        /// Language code - used to store the language in auxiliary functionality
        public string LanguageCode { get; set; }
    }
}
