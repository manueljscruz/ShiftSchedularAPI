using ShiftSchedularEntity.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    /// <summary>
    /// Subscription Duration Type entity represents the different types of subscription durations available for subscription plans. It contains information about the duration type's name, duration in days, whether it applies a promotional discount, and the percentage of the promotional discount if applicable.
    /// </summary>
    public class SubscriptionDurationType : BaseEntity
    {
        /// <summary>
        /// Primary Key - Unique identifier for the Subscription Duration Type.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int SubscriptionDurationTypeId { get; set; }

        /// <summary>
        /// Subscription Duration Type Name - Descriptive name for the subscription duration type (e.g., Monthly, Quarterly, Yearly). This property provides a human-readable label for the duration type, making it easier to identify and select the appropriate duration for subscription plans.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string SubscriptionDurationTypeName { get; set; }

        /// <summary>
        /// Duration in Days - The number of days that the subscription duration type represents. This property is used to calculate the billing period and determine the length of time for which the subscription plan is valid.
        /// </summary>
        [Required]
        public int DurationInDays { get; set; }

        /// <summary>
        /// Flag that indicates whether the subscription duration type applies a promotional discount. If true, the subscription plan associated with this duration type will have a promotional percentage applied to its pricing.
        /// </summary>
        public bool AppliesPromo { get; set; }

        /// <summary>
        /// Promotional Percentage - The percentage of the promotional discount applied to the subscription plan if the AppliesPromo flag is true. This property is used to calculate the discounted price for the subscription plan during the promotional period.
        /// </summary>
        public decimal SubscriptionDurationTypePromoPercentage { get; set; }

        #region Navigation Properties

        /// <summary>
        /// Collection of entities representing the prices associated with different subscription plans for this subscription duration type. This navigation property allows access to the pricing details for each subscription plan that uses this duration type.
        /// </summary>
        public virtual ICollection<SubscriptionPlanDurationPrice> SubscriptionPlanDurationPrices { get; set; }

        /// <summary>
        /// Collection of entities representing the localized names and descriptions for this subscription duration type. This navigation property allows access to the localized information for different languages or regions.
        /// </summary>
        public virtual ICollection<SubscriptionDurationTypeLocalization> SubscriptionDurationTypeLocalizations { get; set; }

        #endregion
    }
}
