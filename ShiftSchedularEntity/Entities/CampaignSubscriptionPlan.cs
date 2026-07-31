using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    /// <summary>
    /// Represents the association between a campaign and a subscription plan duration price, indicating which subscription plans are eligible for a particular campaign.
    /// </summary>
    public class CampaignSubscriptionPlan
    {
        /// <summary>
        /// Composite Key 1 - The unique identifier for the campaign.
        /// </summary>
        [Required]
        public Guid CampaignId { get; set; }

        /// <summary>
        /// Composite Key 2 - The unique identifier for the subscription plan duration price.
        /// </summary>
        [Required]
        public Guid SubscriptionPlanDurationPriceId { get; set; }

        #region Navigation Properties

        /// <summary>
        /// Campaign entity associated with this campaign subscription plan, providing context for the subscription plan's eligibility in the campaign.
        /// </summary>
        public virtual Campaign Campaign { get; set; }

        /// <summary>
        /// Subscription plan duration price entity associated with this campaign subscription plan, indicating the specific subscription plan duration price that is eligible for the campaign.
        /// </summary>
        public virtual SubscriptionPlanDurationPrice SubscriptionPlanDurationPrice { get; set; }

        #endregion
    }
}
