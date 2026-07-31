using ShiftSchedularEntity.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    /// <summary>
    /// Subscription Plan Type entity represents a subscription tier available on the platform (e.g., Free, Trial, Pro, Enterprise). It only carries the tier's identity — pricing, quotas and visibility live on SubscriptionPlanDurationPrice for each tier+duration combination.
    /// </summary>
    public class SubscriptionPlanType : BaseEntity
    {
        /// <summary>
        /// Primary Key - Unique identifier for the Subscription Plan Type.
        /// </summary>
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubscriptionPlanTypeId { get; set; }

        /// <summary>
        /// Internal name of the subscription plan type (e.g., "Basic", "Pro").
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string SubscriptionPlanTypeName { get; set; }

        /// <summary>
        /// Internal description of the subscription plan type, explaining what distinguishes this tier from the others.
        /// </summary>
        [MaxLength(500)]
        public string SubscriptionPlanTypeDescription { get; set; }

        #region Navigation Properties

        /// <summary>
        /// Collection of tier+duration price combinations associated with this subscription plan type.
        /// </summary>
        public virtual ICollection<SubscriptionPlanDurationPrice> SubscriptionPlanDurationPrices { get; set; }

        /// <summary>
        /// Collection of localized names and descriptions for this subscription plan type.
        /// </summary>
        public virtual ICollection<SubscriptionPlanTypeLocalization> SubscriptionPlanTypeLocalizations { get; set; }

        #endregion
    }
}
