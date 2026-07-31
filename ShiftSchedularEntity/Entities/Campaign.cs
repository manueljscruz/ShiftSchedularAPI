using ShiftSchedularEntity.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    /// <summary>
    /// Campaign entity represents a marketing or promotional campaign that can be applied to subscription plans. It contains information about the campaign's name, description, start and end dates, promotion percentage, and other relevant details.
    /// </summary>
    public class Campaign : BaseEntity
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        [Key]
        [Required]
        public Guid CampaignId { get; set; }

        /// <summary>
        /// Name of the Campaign
        /// Ex: Easter, Christmas, Black Friday, etc.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string CampaignName { get; set; }

        /// <summary>
        /// Description of what the campaign is about, its purpose, and any other relevant information.
        /// </summary>
        [MaxLength(500)]
        public string CampaignDescription { get; set; }

        /// <summary>
        /// Start date of the campaign. This is the date when the campaign becomes active and starts running.
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of the campaign. This is the date when the campaign ends and is no longer active, if applicable. If the campaign is ongoing or has no specific end date, this can be null.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Percentage to be applied as a discount or promotion during the campaign. This value should be between 0 and 100, representing the percentage of the discount.
        /// </summary>
        public decimal PromotionPercent { get; set; }

        /// <summary>
        /// Indicates whether the campaign is currently active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Maximum number of times the campaign can be redeemed.
        /// </summary>
        public int MaxRedemptions { get; set; }

        /// <summary>
        /// Current number of times the campaign has been redeemed.
        /// </summary>
        public int RedemptionCount { get; set; }

        /// <summary>
        /// Coupon code associated with the campaign, if applicable.
        /// </summary>
        public string CouponCode { get; set; }

        #region Navigation Properties

        /// <summary>
        /// Collection of Subscription Plans that are eligble for the campaign.
        /// </summary>
        public virtual ICollection<CampaignSubscriptionPlan> CampaignSchedulePlans { get; set; }

        /// <summary>
        /// Collection of Entity Subscription Plans where a campaign was applied. This allows tracking which subscription plans have utilized the campaign for discounts or promotions.
        /// </summary>
        public virtual ICollection<EntitySubscriptionPlan> EntitySubscriptionPlans { get; set; }

        /// <summary>
        /// Collection of Campaign Localizations that provide localized content for the campaign in different languages or regions. This allows the campaign to be presented appropriately to users based on their language or location.
        /// </summary>
        public virtual ICollection<CampaignLocalization> CampaignLocalizations { get; set; }

        #endregion
    }
}
