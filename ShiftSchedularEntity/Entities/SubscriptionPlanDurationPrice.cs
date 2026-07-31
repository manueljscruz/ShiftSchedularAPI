using ShiftSchedularEntity.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    /// <summary>
    /// Subscription Plan Duration Price entity represents the pricing details for a specific subscription plan based on its duration type. It contains information about the base price, scaling options, extra member and generation charges, and whether the plan is public or private.
    /// </summary>
    public class SubscriptionPlanDurationPrice : BaseEntity
    {
        /// <summary>
        /// Primary Key - Unique identifier for the Subscription Plan Duration Price entity. This property serves as the primary key for the entity and is used to uniquely identify each record in the database.
        /// </summary>
        [Required]
        [Key]
        public Guid SubscriptionPlanDurationPriceId { get; set; }

        /// <summary>
        /// Foreign Key - Identifier of the associated Subscription Plan Type. This property establishes a relationship between the Subscription Plan Duration Price and the corresponding subscription plan type, allowing access to the details of the subscription plan.
        /// </summary>
        [Required]
        public int SubscriptionPlanTypeId { get; set; }

        /// <summary>
        /// Foreign Key - Identifier of the associated Subscription Duration Type. This property establishes a relationship between the Subscription Plan Duration Price and the corresponding subscription duration type, allowing access to the details of the subscription duration type.
        /// </summary>
        [Required]
        public int SubscriptionDurationTypeId { get; set; }

        /// <summary>
        /// Base Price - The base price of the subscription plan for the specified duration type. This property represents the standard cost of the subscription without any additional charges for extra members or generations.
        /// </summary>
        public decimal BasePrice { get; set; }

        /// <summary>
        /// Flag indicating whether the subscription plan price scales based on the number of members. If true, the price will increase according to the number of additional members beyond the included ones.
        /// </summary>
        public bool ToScale { get; set; }

        /// <summary>
        /// Scale Requirement - The number of members required to trigger the scaling of the subscription plan price. This property is used in conjunction with the ToScale flag to determine when the price should be adjusted based on the number of members.
        /// </summary>
        public int ScaleRequirement { get; set; }

        /// <summary>
        /// Price Per Extra Member - The additional cost for each member beyond the included ones. This property is used to calculate the total price when the number of members exceeds the included members.
        /// </summary>
        public decimal PricePerExtraMember { get; set; }

        /// <summary>
        /// Included Generations - The number of generations included in the subscription plan. This property represents the base number of generations covered by the subscription without additional charges.
        /// </summary>
        public int IncludedGenerations { get; set; }

        /// <summary>
        /// Price Per Extra Generation - The additional cost for each generation beyond the included ones. This property is used to calculate the total price when the number of generations exceeds the included generations.
        /// </summary>
        public decimal PricePerExtraGeneration { get; set; }

        /// <summary>
        /// Flag indicating whether the subscription plan is a public plan. If true, the plan is available for public subscription.
        /// </summary>
        public bool IsPublicPlan { get; set; }

        /// <summary>
        /// Flag indicating whether the subscription plan is currently active. If true, the plan is available for subscription; if false, it may be inactive or discontinued.
        /// </summary>
        public bool IsActive { get; set; }

        #region Navigation Properties

        /// <summary>
        /// Subscription Duration Type entity associated with this Subscription Plan Duration Price. This navigation property allows access to the details of the subscription duration type, including its name, duration, and promotional information.
        /// </summary>
        public virtual SubscriptionDurationType SubscriptionDurationType { get; set; }
        
        /// <summary>
        /// Subscription Plan Type entity associated with this Subscription Plan Duration Price. This navigation property allows access to the details of the subscription plan type, including its name, features, and pricing information.
        /// </summary>
        public virtual SubscriptionPlanType SubscriptionPlanType { get; set; }

        /// <summary>
        /// Collection of campaigns that have made this subscription plan duration price eligible for a promotion.
        /// </summary>
        public virtual ICollection<CampaignSubscriptionPlan> CampaignSubscriptionPlans { get; set; }

        /// <summary>
        /// Collection of entity subscription plans that were subscribed under this specific plan+duration combination.
        /// </summary>
        public virtual ICollection<EntitySubscriptionPlan> EntitySubscriptionPlans { get; set; }

        #endregion
    }
}
